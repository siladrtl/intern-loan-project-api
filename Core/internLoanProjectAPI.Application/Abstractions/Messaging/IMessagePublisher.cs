using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.Abstractions.Messaging
{
   public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, string queueName);
    }
}
