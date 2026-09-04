using internLoanProjectAPI.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Persistence.Concrete.Services
{
    public class FileStorageService: IFileStorageService
    {
        private readonly string _rootPath;

        public FileStorageService()
        {
            _rootPath = Path.Combine(Directory.GetCurrentDirectory(), "Storage", "VerificationDocuments");
            Directory.CreateDirectory(_rootPath);
        }

        public async Task<string> SaveAsync(Stream fileStream, string fileName, string contentType)
        {
            var extension = Path.GetExtension(fileName);
            var storedFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_rootPath, storedFileName);

            await using var outputStream = new FileStream(filePath, FileMode.Create);
            await fileStream.CopyToAsync(outputStream);

            return filePath;
        }
    }
}



        
    