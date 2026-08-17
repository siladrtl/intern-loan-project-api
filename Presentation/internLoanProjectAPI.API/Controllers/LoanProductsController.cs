using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.DTOs.Product;
using internLoanProjectAPI.Persistence.Concrete.Services;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> GetByLoanType(Guid loanTypeId, Guid customerTypeId)
        {
            var result =
                await _loanProductService
                    .GetByLoanTypeAsync(
                        loanTypeId,
                        customerTypeId);

            return Ok(result);
        }

        //ADMİN 

        [HttpPost("create-loan-product")]
        public async Task<IActionResult> Add(CreateLoanProductRequestDto dto)
        {
            var result = await _loanProductService
                .AddAsync(dto);

            if (!result)
                return BadRequest();

            return Ok("Loan product created successfully.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateLoanProductRequestDto dto)
        {
            var result = await _loanProductService
                .UpdateAsync(id, dto);

            if (!result)
                return NotFound();

            return Ok("Loan product updated successfully.");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _loanProductService
                .DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok("Loan product deleted successfully.");
        }

    }
}



