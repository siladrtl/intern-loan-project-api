using internLoanProjectAPI.Application.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace internLoanProjectAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanTypesController : ControllerBase
    {
        private readonly ILoanTypeService _loanTypeService;

        public LoanTypesController(ILoanTypeService loanTypeService)
        {
            _loanTypeService = loanTypeService;
        }
   
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _loanTypeService.GetAllAsync();

            return Ok(result);
        }

    }
}
