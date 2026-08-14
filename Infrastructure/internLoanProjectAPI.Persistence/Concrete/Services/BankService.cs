using internLoanProject.Domain.Entities;
using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.Abstractions.UnitOfWorks;
using internLoanProjectAPI.Application.DTOs.Common;
using internLoanProjectAPI.Application.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Persistence.Concrete.Services
{
    public class BankService : IBankService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BankService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<BankDto>> GetAllAsync()
        {
            return await _unitOfWork
                .GetReadRepository<Bank>()
                .GetAll(false)
                .Select(bank => new BankDto
                {
                    Id = bank.Id,
                    Name = bank.Name
                })
                .ToListAsync();
        }
    }
}
    
