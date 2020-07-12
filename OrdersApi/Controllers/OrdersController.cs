using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrdersApi.Models;
using OrdersApi.Persistence.Repository;

namespace OrdersApi.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
      
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var resultFromRepo = await _orderRepository.GetAllOrderAsyc();
            return Ok(resultFromRepo);
        }
        [HttpGet("{orderId}",Name = "GetOrderById")]
        public async Task<IActionResult> GetOrderById(string orderId)
        {
            var resultFromRepo = await _orderRepository.GetOrderAsycById(Guid.Parse(orderId));
            if (resultFromRepo == null) return NotFound();
            return Ok(resultFromRepo);
        }
    }
}
