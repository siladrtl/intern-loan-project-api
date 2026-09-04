using FluentValidation;
using internLoanProjectAPI.API.Models;
using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace internLoanProjectAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<RegisterRequestDto> _registerValidator;
        private readonly IValidator<RegisterFormRequest> _registerFormValidator;

        public AuthController(IAuthService authService, IValidator<RegisterRequestDto> registerValidator, IValidator<RegisterFormRequest> registerFormValidator)
        {
            _authService = authService;
            _registerValidator = registerValidator;
            _registerFormValidator = registerFormValidator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterFormRequest request)
        {
            var formValidationResult = await _registerFormValidator.ValidateAsync(request);

            if (!formValidationResult.IsValid)
            {
                return BadRequest(new
                {
                    errors = formValidationResult.Errors.Select(x => x.ErrorMessage).ToList()
                });
            }

            var registerDto = new RegisterRequestDto
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                BirthDate = request.BirthDate,
                NationalId = request.NationalId,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                City = request.City,
                District = request.District,
                CustomerType = request.CustomerType,
                Password = request.Password
            };

            var validationResult = await _registerValidator.ValidateAsync(registerDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList()
                });
            }

            VerificationDocumentDto? verificationDocument = null;

            if (request.VerificationDocument != null && request.VerificationDocument.Length > 0)
            {
                verificationDocument = new VerificationDocumentDto
                {
                    FileStream = request.VerificationDocument.OpenReadStream(),
                    FileName = request.VerificationDocument.FileName,
                    ContentType = request.VerificationDocument.ContentType,
                    FileSize = request.VerificationDocument.Length
                };
            }

            try
            {
                var result = await _authService.RegisterAsync(registerDto, verificationDocument);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}