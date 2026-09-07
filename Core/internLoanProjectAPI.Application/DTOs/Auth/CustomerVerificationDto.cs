using internLoanProject.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.DTOs.Auth
{
    public class CustomerVerificationDto
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public CustomerType CustomerType { get; set; }

        public string OriginalFileName { get; set; }

        public string ContentType { get; set; }

        public long FileSize { get; set; }

        public VerificationStatus Status { get; set; }

        public DateTime UploadedAt { get; set; }

        public DateTime? VerifiedAt { get; set; }

        public string? VerificationNote { get; set; }
    }
}
