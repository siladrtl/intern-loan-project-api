using internLoanProject.Domain.Entities.Enums;

namespace internLoanProjectAPI.API.Models
{
    public class RegisterFormRequest
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public DateTime BirthDate { get; set; }

        public string NationalId { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string City { get; set; } = null!;

        public string District { get; set; } = null!;

        public CustomerType CustomerType { get; set; }

        public string Password { get; set; } = null!;

        public IFormFile VerificationDocument { get; set; } = null!;
    }
}
