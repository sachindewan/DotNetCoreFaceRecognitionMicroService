using Faces.WebMvc.Models;
using Microsoft.Extensions.Configuration;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Faces.WebMvc.RestClients
{
    public class OrderApiManagement : IOrderManagementApi
    {
        private readonly IOrderManagementApi _restClient;
        public OrderApiManagement(IConfiguration configuration,HttpClient client)
        {
            string apiHostAndPort = configuration.GetSection("ApiServiceLocation").GetValue<string>("OrderApiLocation");
            client.BaseAddress = new Uri($"http://{apiHostAndPort}/api");
            _restClient = RestService.For<IOrderManagementApi>(client);
        }
        public async Task<OrderViewModel> GetOrderById(Guid orderId)
        {
            try
            {
                return await _restClient.GetOrderById(orderId);
            }
            catch(ApiException exception)
            {
                if(exception.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<List<OrderViewModel>> GetOrders()
        {
            return await _restClient.GetOrders();
        }
    }
}
