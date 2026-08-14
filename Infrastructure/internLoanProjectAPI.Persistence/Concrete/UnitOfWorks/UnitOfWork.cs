using internLoanProject.Domain.Entities.Common;
using internLoanProjectAPI.Application.Abstractions.UnitOfWorks;
using internLoanProjectAPI.Application.Repositories;
using internLoanProjectAPI.Persistence.Contexts;
using internLoanProjectAPI.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Persistence.Concrete.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly internLoanProjectAPIDbContext _context;
        public UnitOfWork(internLoanProjectAPIDbContext context)
        {
                _context = context;
        }
        public async ValueTask DisposeAsync() => await _context.DisposeAsync();
        public int Save() => _context.SaveChanges();
        public Task<int> SaveAsync() => _context.SaveChangesAsync();

        public IReadRepository<T> GetReadRepository<T>() where T : BaseEntity, new() => new ReadRepository<T>(_context);
      
        public IWriteRepository<T> GetWriteRepository<T>() where T : BaseEntity, new() => new WriteRepository<T>(_context);
       
    }
}
