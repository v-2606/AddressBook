using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQLayer.Interface
{
     public interface  IEventPublisher
    {
        void PublishEvent(string queueName, object message);
    }
}
