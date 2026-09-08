using internLoanProjectAPI.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace internLoanProjectAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
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

        [HttpGet("{id}/document")]
        public async Task<IActionResult> GetDocument(int id)
        {
            try
            {
                var document = await _verificationService.GetDocumentAsync(id);

                if (!System.IO.File.Exists(document.FilePath))
                {
                    return NotFound(new
                    {
                        message = "Belge dosyası sunucuda bulunamadı."
                    });
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(document.FilePath);

                return File(
                    fileBytes,
                    document.ContentType,
                    document.OriginalFileName
                );
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(
            int id,
            [FromBody] string? note)
        {
            try
            {
                var result = await _verificationService.ApproveAsync(id, note);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
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
                var result = await _verificationService.RejectAsync(id, note);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message
                });
            }
        }
    }
}