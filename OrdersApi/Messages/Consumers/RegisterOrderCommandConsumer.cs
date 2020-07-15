using MassTransit;
using MassTransit.Configuration;
using Messaging.InterfacesConstants.Commands;
using Messaging.InterfacesConstants.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrdersApi.Hubs;
using OrdersApi.Models;
using OrdersApi.Persistence.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OrdersApi.Messages.Consumers
{
    public class RegisterOrderCommandConsumer : IConsumer<IRegisterOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly IOptions<OrderSettings> _options;

        public RegisterOrderCommandConsumer(IOrderRepository orderRepository,IHttpClientFactory httpClientFactory, IHubContext<OrderHub> hubContext,IOptions<OrderSettings> options)
        {
            _orderRepository = orderRepository;
            _httpClientFactory = httpClientFactory;
            _hubContext = hubContext;
            _options = options;
        }
        public async Task Consume(ConsumeContext<IRegisterOrderCommand> context)
        {
            if(context.Message.ImageData!=null && context.Message.OrderId!=null && context.Message.UserEmail != null)
            {
               await SaveOrderData(context.Message);
                await _hubContext.Clients.All.SendAsync("UpdateOrders", " New order createed", context.Message.OrderId);
               var client = _httpClientFactory.CreateClient();
                Tuple<List<byte[]>, Guid> orderDetailsData = await GetFacesFromFaceApiAsync(client, context.Message.ImageData,context.Message.OrderId);
                List<byte[]> faces = orderDetailsData.Item1;
                Guid orderId = orderDetailsData.Item2;
                await SaveOrderDetails(orderId, faces);
                await _hubContext.Clients.All.SendAsync("UpdateOrders", " Order processed", context.Message.OrderId);
                //publishing order processed event

                await context.Publish<IOrderProcessedEvent>(new
                {
                    OrderId = orderId,
                    context.Message.UserEmail,
                    Faces = faces,
                    context.Message.ImageUrl
                });
            }
        }

        private async Task<Tuple<List<byte[]>, Guid>> GetFacesFromFaceApiAsync(HttpClient client,byte[] imageData,Guid orderId)
        {
            var byteContentArray = new ByteArrayContent(imageData);
            Tuple<List<byte[]>, Guid> orderDetailData = null;
            byteContentArray.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            using(var response = await client.PostAsync(_options.Value.FacesApiUrl+ orderId, byteContentArray))
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                orderDetailData = JsonConvert.DeserializeObject<Tuple<List<byte[]>, Guid>>(apiResponse);
            }
            return orderDetailData;
        }

        private async Task<bool> SaveOrderDetails(Guid orderId, List<byte[]> faces)
        {
            var orderFromRepo = await _orderRepository.GetOrderAsycById(Guid.Parse(orderId.ToString()));
            if (orderFromRepo != null)
            {
                orderFromRepo.Status = Status.Processed;
                foreach (var face in faces)
                {
                    OrderDetail orderDetail = new OrderDetail() { FaceData = face, OrderId = orderFromRepo.OrderId };
                    orderFromRepo.OrderDetails.Add(orderDetail);
                }
                //_orderRepository.UpdateOrder(orderFromRepo);
            }
            return await _orderRepository.SaveAllAsync();

        }

        private  async Task SaveOrderData(IRegisterOrderCommand message)
        {
            Order order = new Order { 
                ImageData = message.ImageData,
                OrderId = message.OrderId,
                UserEmail = message.UserEmail,
                Status = Status.Registred,
                PictureUrl = message.ImageUrl
            };
            await _orderRepository.AddOrderEntity(order);
        }
    }
}
