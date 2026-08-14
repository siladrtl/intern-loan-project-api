using internLoanProjectAPI.Application.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace internLoanProjectAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BanksController : ControllerBase
    {
        private readonly IBankService _bankService;

        public BanksController(IBankService bankService)
        {
            _bankService = bankService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _bankService.GetAllAsync();
            return Ok(result);
        }
    }
}
