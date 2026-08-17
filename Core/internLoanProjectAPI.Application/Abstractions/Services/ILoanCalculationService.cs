using internLoanProjectAPI.Application.DTOs.Calculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.Abstractions.Services
{
    public interface ILoanCalculationService
    {
        Task<LoanCalculationDto> CalculateAsync(CreateLoanCalculationDto dto);
    }
}
