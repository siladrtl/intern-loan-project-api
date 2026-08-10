using internLoanProjectAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Persistence
{
    public class DesingTimeDbContextFactory: IDesignTimeDbContextFactory<internLoanProjectAPIDbContext>
    {
        public internLoanProjectAPIDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<internLoanProjectAPIDbContext> dbContextOptionsBuilder = new();
            dbContextOptionsBuilder.UseSqlServer(Configuration.ConnectionString);
            return new(dbContextOptionsBuilder.Options);
        }
    }
}
