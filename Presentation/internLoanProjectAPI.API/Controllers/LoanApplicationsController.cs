using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.DTOs.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace internLoanProjectAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class LoanApplicationsController : ControllerBase
    {
        private readonly ILoanApplicationService _loanApplicationService;

        public LoanApplicationsController(ILoanApplicationService loanApplicationService)
        {
            _loanApplicationService = loanApplicationService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLoanApplicationDto dto)
        {
            try
            {
                var result = await _loanApplicationService.CreateAsync(dto);

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
        [HttpGet("my-applications")]
        public async Task<IActionResult> GetMyApplications()
        {
            try
            {
                var result =
                    await _loanApplicationService
                        .GetMyApplicationsAsync();

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
