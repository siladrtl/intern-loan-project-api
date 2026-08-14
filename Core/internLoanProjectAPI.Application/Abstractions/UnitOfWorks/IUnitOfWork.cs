using internLoanProject.Domain.Entities.Common;
using internLoanProjectAPI.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.Abstractions.UnitOfWorks
{
    public interface IUnitOfWork: IAsyncDisposable
    {
        IReadRepository<T> GetReadRepository<T>() where T : BaseEntity, new();
        IWriteRepository<T> GetWriteRepository<T>() where T : BaseEntity, new();
        Task<int> SaveAsync();
        int Save();

    }
}
