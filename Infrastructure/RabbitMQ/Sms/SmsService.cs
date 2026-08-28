using internLoanProjectAPI.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.RabbitMQ.Sms
{
    public class SmsService: ISmsService
    {
        public Task SendAsync(string phoneNumber, string message)
        {
            Console.WriteLine($"SMS -> {phoneNumber}: {message}");
            return Task.CompletedTask;
        }
    }
}
