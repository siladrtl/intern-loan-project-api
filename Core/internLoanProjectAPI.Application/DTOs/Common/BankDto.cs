using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.DTOs.Common
{
    public class BankDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
