using internLoanProjectAPI.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.Abstractions.Services
{
    public interface IBankService
    {
        Task<List<BankDto>> GetAllAsync();
    }
}
