using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.DTOs.Calculation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace internLoanProjectAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanCalculationsController : ControllerBase
    {
        private readonly ILoanCalculationService _loanCalculationService;

        public LoanCalculationsController(ILoanCalculationService loanCalculationService)
        {
            _loanCalculationService = loanCalculationService;
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> Calculate(
        CreateLoanCalculationDto dto)
        {
            var result =
                await _loanCalculationService
                    .CalculateAsync(dto);

            return Ok(result);
        }
    }
}
