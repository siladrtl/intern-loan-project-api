using internLoanProjectAPI.Application.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace internLoanProjectAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminCustomerVerificationController : ControllerBase
    {
        private readonly IAdminCustomerVerificationService _verificationService;

        public AdminCustomerVerificationController(
            IAdminCustomerVerificationService verificationService)
        {
            _verificationService = verificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _verificationService.GetAllAsync();

            return Ok(result);
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(
            int id,
            [FromBody] string? note)
        {
            try
            {
                var result = await _verificationService
                    .ApproveAsync(id, note);

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

        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(
            int id,
            [FromBody] string? note)
        {
            try
            {
                var result = await _verificationService
                    .RejectAsync(id, note);

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



