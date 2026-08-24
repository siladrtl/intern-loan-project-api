using FluentValidation;

using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.DTOs.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace internLoanProjectAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<RegisterRequestDto> _registerValidator;


        public AuthController(
            IAuthService authService,
            IValidator<RegisterRequestDto> registerValidator)
        {
            _authService = authService;
            _registerValidator = registerValidator;
        }

        // REGISTER
 

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequestDto request)
        {
            var validationResult =
                await _registerValidator
                    .ValidateAsync(request);


            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    errors =
                        validationResult.Errors
                            .Select(
                                x => x.ErrorMessage
                            )
                            .ToList()
                });
            }


            try
            {
                var result =
                    await _authService
                        .RegisterAsync(request);

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

        // LOGIN
      

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequestDto request)
        {
            try
            {
                var result =
                    await _authService
                        .LoginAsync(request);

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