using internLoanProjectAPI.Application.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace internLoanProjectAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerVerificationController : ControllerBase
    {
        private readonly ICustomerVerificationService _verificationService;

        public CustomerVerificationController(ICustomerVerificationService verificationService)
        {
            _verificationService = verificationService;
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetMyStatus()
        {
            try
            {
                var result =
                    await _verificationService.GetMyStatusAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}

