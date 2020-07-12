using OrdersApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersApi.Persistence.Repository
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderAsycById(Guid Id);

        Task<IEnumerable<Order>> GetAllOrderAsyc();

        Order GetOrderId(Guid Id);

        IEnumerable<Order> GetAllOrder(Guid Id);

        Task<bool> SaveAllAsync();

        Task<bool> AddOrderEntity(Order order);

        Task<bool> UpdateOrder(Order order);
    }
}
