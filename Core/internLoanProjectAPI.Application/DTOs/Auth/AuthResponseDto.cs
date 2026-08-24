using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; }

        public string Email { get; set; }

        public int? CustomerId { get; set; }
    }
}
