using Faces.WebMvc.Models;
using Faces.WebMvc.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
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
        private readonly IOptions<AppSettings> _options;
        public OrderApiManagement(IConfiguration configuration,HttpClient client,IOptions<AppSettings> options)
        {
            _options = options;
            client.BaseAddress = new Uri(_options.Value.OrdersApiUrl);
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
