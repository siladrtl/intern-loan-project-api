using internLoanProject.Domain.Entities;
using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.Abstractions.UnitOfWorks;
using internLoanProjectAPI.Application.DTOs.CustomerType;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Persistence.Concrete.Services
{
    public class CustomerTypeService: ICustomerTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CustomerTypeDto>> GetAllAsync()
        {
            return await _unitOfWork
                .GetReadRepository<CustomerType>()
                .GetAll(false)
                .Select(x => new CustomerTypeDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();
        }
    }
}
