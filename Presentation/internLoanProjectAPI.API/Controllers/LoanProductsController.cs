using internLoanProject.Domain.Entities.Enums;
using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.DTOs.Product;
using Microsoft.AspNetCore.Mvc;

namespace internLoanProjectAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanProductsController : ControllerBase
    {
        private readonly ILoanProductService _loanProductService;

        public LoanProductsController(ILoanProductService loanProductService)
        {
            _loanProductService = loanProductService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _loanProductService.GetAllAsync();

            return Ok(result);
        }

        [HttpGet("by-loan-type")]
        public async Task<IActionResult> GetByLoanType(
            int loanTypeId,
            CustomerType customerType)
        {
            var result =
                await _loanProductService
                    .GetByLoanTypeAsync(
                        loanTypeId,
                        customerType);

            return Ok(result);
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(
            [FromBody] ProductSearchRequestDto dto)
        {
            var result =
                await _loanProductService
                    .SearchAsync(dto);

            return Ok(result);
        }
    }
}