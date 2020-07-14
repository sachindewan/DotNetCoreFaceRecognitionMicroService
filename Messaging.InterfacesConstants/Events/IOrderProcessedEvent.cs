using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.InterfacesConstants.Events
{
    public interface IOrderProcessedEvent
    {
        Guid OrderId { get; }
        string ImageUrl { get; }
        List<byte[]> Faces { get; }
        string UserEmail { get; }
    }
}
