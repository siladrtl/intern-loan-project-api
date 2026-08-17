using internLoanProjectAPI.Application.Abstractions.Services;
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
    }
}
