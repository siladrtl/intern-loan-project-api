using internLoanProjectAPI.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace internLoanProjectAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RabbitMQTestsController : ControllerBase
    {
        private readonly IMessagePublisher _messagePublisher;

        public RabbitMQTestsController(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }
        [HttpPost]
        public async Task<IActionResult> SendTestMessage()
        {
            var message =
                new
                {
                    ApplicationId = 1,
                    CustomerId = 3,
                    Status = "Pending"
                };


            await _messagePublisher
                .PublishAsync(
                    message,
                    "loan-application-queue"
                );


            return Ok(
                new
                {
                    message =
                        "RabbitMQ mesajı gönderildi."
                }
            );
        }
    }
}
