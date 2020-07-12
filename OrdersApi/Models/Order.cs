using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersApi.Models
{
    public class Order
    {
        public Order()
        {
            OrderDetails = new System.Collections.ObjectModel.Collection<OrderDetail>();
        }
        public Guid OrderId { get; set; }
        public string PictureUrl { get; set; }
        public byte[] ImageData { get; set; }
        public string UserEmail { get; set; }
        public Status Status { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
