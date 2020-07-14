using MassTransit;
using Messaging.InterfacesConstants.Events;
using Microsoft.AspNetCore.SignalR;
using OrdersApi.Hubs;
using OrdersApi.Persistence.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersApi.Messages.Consumers
{
    public class OrderDispatchedEventConsumer : IConsumer<IOrderDispatchedEvent>
    {
        private readonly IOrderRepository _repo;
        private readonly IHubContext<OrderHub> _hubContext;
        public OrderDispatchedEventConsumer(IOrderRepository orderRepository,IHubContext<OrderHub> hubContext)
        {
            _repo = orderRepository;
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<IOrderDispatchedEvent> context)
        {
            var result = context.Message;
            var orderDetailsFromRepo = await _repo.GetOrderAsycById(result.OrderId);
            if (orderDetailsFromRepo != null)
            {
                orderDetailsFromRepo.Status = Models.Status.Sent;
                await _repo.UpdateOrder(orderDetailsFromRepo);
            }
            await _hubContext.Clients.All.SendAsync("UpdateOrders", "Dispatched", result.OrderId);
            //throw new NotImplementedException();
        }
    }
}
