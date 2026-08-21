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

        public LoanProductsController(
            ILoanProductService loanProductService)
        {
            _loanProductService = loanProductService;
        }


        // ==========================================
        // TÜM AKTİF ÜRÜNLER
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _loanProductService.GetAllAsync();

            return Ok(result);
        }


        // ==========================================
        // KREDİ TÜRÜ + MÜŞTERİ TİPİ
        // ==========================================

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


        // ==========================================
        // ÜRÜN ARAMA
        // ==========================================

        [HttpPost("Search")]
        public async Task<IActionResult> Search(
            [FromBody] ProductSearchRequestDto dto)
        {
            var result =
                await _loanProductService
                    .SearchAsync(dto);

            return Ok(result);
        }


        // ==========================================
        // ADMIN - ÜRÜN EKLE
        // ==========================================

        [HttpPost("create-loan-product")]
        public async Task<IActionResult> Add(
            [FromBody] CreateLoanProductRequestDto dto)
        {
            var result =
                await _loanProductService
                    .AddAsync(dto);

            if (!result)
            {
                return BadRequest();
            }

            return Ok(
                "Kredi ürünü başarılı şekilde oluşturuldu.");
        }


        // ==========================================
        // ADMIN - ÜRÜN GÜNCELLE
        // ==========================================

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateLoanProductRequestDto dto)
        {
            var result =
                await _loanProductService
                    .UpdateAsync(id, dto);

            if (!result)
            {
                return NotFound();
            }

            return Ok(
                "Kredi ürünü başarılı şekilde güncellendi.");
        }


        // ==========================================
        // ADMIN - ÜRÜN PASİFLEŞTİR
        // ==========================================

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            int id)
        {
            var result =
                await _loanProductService
                    .DeleteAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return Ok(
                "Kredi ürünü başarılı şekilde silindi.");
        }
    }
}