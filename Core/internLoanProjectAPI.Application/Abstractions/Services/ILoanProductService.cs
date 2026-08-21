using internLoanProject.Domain.Entities.Enums;
using internLoanProjectAPI.Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.Abstractions.Services
{
    public interface ILoanProductService
    {
        // Kullanıcı + Admin
        Task<List<LoanProductDto>> GetAllAsync();


        Task<List<LoanProductDto>> GetByLoanTypeAsync(
            int loanTypeId,
            CustomerType customerType);


        Task<List<ProductSearchResultDto>> SearchAsync(
            ProductSearchRequestDto request);


        // Admin
        Task<bool> AddAsync(
            CreateLoanProductRequestDto dto);


        Task<bool> UpdateAsync(
            int id,
            UpdateLoanProductRequestDto dto);


        Task<bool> DeleteAsync(
            int id);
    }
}


