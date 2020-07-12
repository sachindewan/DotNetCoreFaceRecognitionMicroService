using MassTransit;
using Messaging.InterfacesConstants.Commands;
using Newtonsoft.Json;
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

        public RegisterOrderCommandConsumer(IOrderRepository orderRepository,IHttpClientFactory httpClientFactory)
        {
            _orderRepository = orderRepository;
            _httpClientFactory = httpClientFactory;
        }
        public async Task Consume(ConsumeContext<IRegisterOrderCommand> context)
        {
            if(context.Message.ImageData!=null && context.Message.OrderId!=null && context.Message.UserEmail != null)
            {
               SaveOrderData(context.Message);
               var client = _httpClientFactory.CreateClient();
                Tuple<List<byte[]>, Guid> orderDetailsData = await GetFacesFromFaceApiAsync(client, context.Message.ImageData,context.Message.OrderId);
                List<byte[]> faces = orderDetailsData.Item1;
                Guid orderId = orderDetailsData.Item2;
                await SaveOrderDetails(orderId, faces);
            }
        }

        private async Task<Tuple<List<byte[]>, Guid>> GetFacesFromFaceApiAsync(HttpClient client,byte[] imageData,Guid orderId)
        {
            var byteContentArray = new ByteArrayContent(imageData);
            Tuple<List<byte[]>, Guid> orderDetailData = null;
            byteContentArray.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            using(var response = await client.PostAsync("http://localhost:6001/api/faces?orderId=" + orderId, byteContentArray))
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

        private  void SaveOrderData(IRegisterOrderCommand message)
        {
            Order order = new Order { 
                ImageData = message.ImageData,
                OrderId = message.OrderId,
                UserEmail = message.UserEmail,
                Status = Status.Registred,
                PictureUrl = message.ImageUrl
            };
            _orderRepository.AddOrderEntity(order);
        }
    }
}
