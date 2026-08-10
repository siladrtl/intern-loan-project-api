using internLoanProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Persistence.Contexts
{
    public class internLoanProjectAPIDbContext : DbContext
    {
        public internLoanProjectAPIDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<LoanProduct> LoanProducts { get; set; }
        public DbSet<LoanType> LoanTypes { get; set; }
        public DbSet<LoanCalculation> LoanCalculations { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<PaymentPlan> PaymentPlans { get; set; }


    }
}
