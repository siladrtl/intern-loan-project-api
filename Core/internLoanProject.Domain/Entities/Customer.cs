using internLoanProject.Domain.Entities.Common;
using internLoanProject.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProject.Domain.Entities
{
    public class Customer: BaseEntity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public string NationalId { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public CustomerType CustomerType { get; set; }

        public ICollection<LoanApplication> LoanApplications { get; set; }

        public CustomerVerificationDocument? VerificationDocument { get; set; }
    }
}
