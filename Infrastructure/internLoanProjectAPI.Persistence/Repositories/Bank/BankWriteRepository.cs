using internLoanProject.Domain.Entities;
using internLoanProjectAPI.Application.Repositories;
using internLoanProjectAPI.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Persistence.Repositories
{
    public class BankWriteRepository : WriteRepository<Bank>, IBankWriteRepository
    {
        public BankWriteRepository(internLoanProjectAPIDbContext context) : base(context)
        {
        }
    }
}
