using Microsoft.EntityFrameworkCore;
using OrdersApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersApi.Persistence.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;
        public OrderRepository(OrderDbContext orderDbContext)
        {
            _context = orderDbContext;
        }

        public async Task<bool> AddOrderEntity(Order order)
        {
            _context.Orders.Add(order);
             var result = await SaveAllAsync();
            return result;
        }

        public IEnumerable<Order> GetAllOrder(Guid Id)
        {
            return _context.Orders.ToList();
        }

        public async Task<IEnumerable<Order>> GetAllOrderAsyc()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<Order> GetOrderAsycById(Guid Id)
        {
            return await _context.Orders.FirstOrDefaultAsync(x=>x.OrderId == Id);
        }

        public Order GetOrderId(Guid Id)
        {
            return  _context.Orders.Where(x => x.OrderId == Id).FirstOrDefault();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
            return await SaveAllAsync();
        }
    }
}
