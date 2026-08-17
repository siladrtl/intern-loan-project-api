using internLoanProject.Domain.Entities;
using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.Abstractions.UnitOfWorks;
using internLoanProjectAPI.Application.DTOs.Common;
using internLoanProjectAPI.Persistence.Concrete.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Persistence.Concrete.Services
{
    public class LoanTypeService : ILoanTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LoanTypeService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public async Task<List<LoanTypeDto>> GetAllAsync()
        {
            return await _unitOfWork
                .GetReadRepository<LoanType>()
                .GetAll(false)
                .Select(x => new LoanTypeDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();
        }

    }
}
