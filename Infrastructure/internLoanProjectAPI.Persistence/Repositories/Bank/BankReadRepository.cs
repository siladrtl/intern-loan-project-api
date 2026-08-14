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
    public class BankReadRepository: ReadRepository<Bank>, IBankReadRepository
    {
        public BankReadRepository(internLoanProjectAPIDbContext context) : base(context)
        {
        }
    }
}
