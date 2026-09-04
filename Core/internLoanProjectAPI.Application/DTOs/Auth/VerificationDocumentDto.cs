using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.DTOs.Auth
{
    public class VerificationDocumentDto
    {
        public Stream FileStream { get; set; } = null!;

        public string FileName { get; set; } = null!;

        public string ContentType { get; set; } = null!;

        public long FileSize { get; set; }
    }
}
