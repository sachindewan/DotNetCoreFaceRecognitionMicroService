using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Faces.WebMvc.Models;
using MassTransit;
using System.IO;
using Messaging.InterfacesConstants.Constants;
using Messaging.InterfacesConstants.Commands;
using Faces.WebMvc.RestClients;

namespace Faces.WebMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBusControl _busControl;
        public HomeController(ILogger<HomeController> logger, IBusControl busControl)
        {
            _busControl = busControl;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult RegisterOrder()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterOrder(OrderViewModel orderViewModel)
        {
            MemoryStream stream = new MemoryStream();
            if (orderViewModel.File==null || orderViewModel.File.Length == 0)
            {
                return BadRequest();
            }
            using(var uplodedFile = orderViewModel.File.OpenReadStream())
            {
                await uplodedFile.CopyToAsync(stream);
            }
            orderViewModel.ImageData = stream.ToArray();
            orderViewModel.ImageUrl = orderViewModel.File.FileName;
            orderViewModel.OrderId = Guid.NewGuid();
            var sendToUri = new Uri($"{RabbitMqMassTransitContstants.RabbitMquri}" + $"{ RabbitMqMassTransitContstants.RegisterOrderCommandQueue}");
            var endPoint = await _busControl.GetSendEndpoint(sendToUri);
            await endPoint.Send<IRegisterOrderCommand>(new
            {
                orderViewModel.OrderId,
                orderViewModel.ImageData,
                orderViewModel.UserEmail,
                orderViewModel.ImageUrl
            });
            ViewBag.OrderId = orderViewModel.OrderId;
            return View("Thanks");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
