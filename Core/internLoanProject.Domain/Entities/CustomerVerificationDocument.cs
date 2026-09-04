using internLoanProject.Domain.Entities.Common;
using internLoanProject.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProject.Domain.Entities
{
    public class CustomerVerificationDocument: BaseEntity
    {
        public int CustomerId { get; set; }

        public Customer Customer { get; set; } = null!;

        public string OriginalFileName { get; set; } = null!;

        public string StoredFileName { get; set; } = null!;

  
        public string ContentType { get; set; } = null!;

 
        public long FileSize { get; set; }

     
        public string FilePath { get; set; } = null!;

        
        public VerificationStatus Status { get; set; } = VerificationStatus.Pending;

        public DateTime UploadedAt { get; set; } = DateTime.Now;

        public DateTime? VerifiedAt { get; set; }

        public string? VerificationNote { get; set; }
    }
}
