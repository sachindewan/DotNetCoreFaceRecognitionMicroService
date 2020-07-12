using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersApi.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }
        public Byte[] FaceData { get; set; }
    }
}
