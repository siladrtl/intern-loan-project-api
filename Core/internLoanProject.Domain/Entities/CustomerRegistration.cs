using internLoanProject.Domain.Entities.Common;
using internLoanProject.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProject.Domain.Entities
{
    public class CustomerRegistration: BaseEntity
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

        public VerificationStatus Status { get; set; }
            = VerificationStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? VerifiedAt { get; set; }

        public string? VerificationNote { get; set; }

        public CustomerVerificationDocument? VerificationDocument { get; set; }
    }
}
