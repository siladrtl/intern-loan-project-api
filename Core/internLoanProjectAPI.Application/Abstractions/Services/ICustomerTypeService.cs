using internLoanProjectAPI.Application.DTOs.CustomerType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.Abstractions.Services
{
    public interface ICustomerTypeService
    {
        Task<List<CustomerTypeDto>> GetAllAsync();
    }
}
