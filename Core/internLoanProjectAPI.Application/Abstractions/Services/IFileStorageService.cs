using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.Abstractions.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveAsync(Stream fileStream, string fileName, string contentType);
    }
}
