using internLoanProjectAPI.Application.DTOs.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.Abstractions.Services
{
    public interface ILoanApplicationService
    {
        Task<LoanApplicationDto> CreateAsync(CreateLoanApplicationDto dto);
        
    }
}
