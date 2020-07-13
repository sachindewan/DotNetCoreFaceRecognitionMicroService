using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Faces.WebMvc.RestClients;
using Faces.WebMvc.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Faces.WebMvc.Controllers
{
    public class OrderManagementController : Controller
    {
        private readonly IOrderManagementApi _orderManagementApi;
        public OrderManagementController(IOrderManagementApi orderManagementApi)
        {
            _orderManagementApi = orderManagementApi;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new OrderListViewModel
            {
                Orders = await _orderManagementApi.GetOrders()

            };
            foreach (var order in model.Orders)
            {
                order.ImageString = ConvertAndFormatToString(order.ImageData);
            }
            return View(model);
        }

        public async Task<IActionResult> Details(Guid orderId)
        {
            var orderDetailsFromApi = await _orderManagementApi.GetOrderById(orderId);
            foreach(var order in orderDetailsFromApi.OrderDetails)
            {
                order.ImageString = ConvertAndFormatToString(order.FaceData);
            }
            return View(orderDetailsFromApi);
        }
        #region private helpers
        private string ConvertAndFormatToString(byte[] orderData)
        {
            var base64String = Convert.ToBase64String(orderData);
            return string.Format("data:image/png;base64,{0}", base64String);

        }
        #endregion
    }
}
