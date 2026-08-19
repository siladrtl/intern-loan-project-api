using internLoanProject.Domain.Entities;
using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.Abstractions.UnitOfWorks;
using internLoanProjectAPI.Application.DTOs.Product;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Persistence.Concrete.Services
{
    public class LoanProductService : ILoanProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LoanProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<LoanProductDto>> GetAllAsync()
        {
            return await _unitOfWork
                .GetReadRepository<LoanProduct>()
                .GetAll(false)
                .Where(x => x.IsActive)
                .Select(x => new LoanProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    InterestRate = x.InterestRate,
                    MinAmount = x.MinAmount,
                    MaxAmount = x.MaxAmount,
                    MinTerm = x.MinTerm,
                    MaxTerm = x.MaxTerm,

                    BankId = x.BankId,
                    BankName = x.Bank.Name,

                    LoanTypeId = x.LoanTypeId,
                    LoanTypeName = x.LoanType.Name
                })
                .ToListAsync();
        }

        public async Task<List<LoanProductDto>>
            GetByLoanTypeAsync(
                Guid loanTypeId,
                Guid customerTypeId)
        {
            return await _unitOfWork
                .GetReadRepository<LoanProduct>()
                .GetWhere(
                    x =>
                        x.LoanTypeId == loanTypeId &&
                        x.CustomerTypeId == customerTypeId &&
                        x.IsActive,
                    false)
                .Select(x => new LoanProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    InterestRate = x.InterestRate,
                    MinAmount = x.MinAmount,
                    MaxAmount = x.MaxAmount,
                    MinTerm = x.MinTerm,
                    MaxTerm = x.MaxTerm,

                    BankId = x.BankId,
                    BankName = x.Bank.Name,

                    LoanTypeId = x.LoanTypeId,
                    LoanTypeName = x.LoanType.Name
                })
                .ToListAsync();
        }
        // ADMIN - ADD
        public async Task<bool> AddAsync(
            CreateLoanProductRequestDto x)
        {
            var loanProduct = new LoanProduct
            {
                Id = Guid.NewGuid(),

                Name = x.Name,

                InterestRate = x.InterestRate,

                MinAmount = x.MinAmount,
                MaxAmount = x.MaxAmount,

                MinTerm = x.MinTerm,
                MaxTerm = x.MaxTerm,

                BankId = x.BankId,

                LoanTypeId = x.LoanTypeId,

                CustomerTypeId = x.CustomerTypeId,

                IsActive = true
            };

            var result = await _unitOfWork
                .GetWriteRepository<LoanProduct>()
                .AddAsync(loanProduct);

            if (!result)
                return false;

            await _unitOfWork.SaveAsync();

            return true;
        }

        // ADMIN - UPDATE
        public async Task<bool> UpdateAsync(
            Guid id,
            UpdateLoanProductRequestDto x)
        {
            var loanProduct =
                await _unitOfWork
                    .GetReadRepository<LoanProduct>()
                    .GetSingleAsync(x => x.Id == id);

            if (loanProduct == null)
                return false;

            loanProduct.Name = x.Name;
            loanProduct.InterestRate = x.InterestRate;
            loanProduct.MinAmount = x.MinAmount;
            loanProduct.MaxAmount = x.MaxAmount;
            loanProduct.MinTerm =  x.MinTerm;
            loanProduct.MaxTerm = x.MaxTerm;
            loanProduct.BankId =  x.BankId;
            loanProduct.LoanTypeId = x.LoanTypeId;
            loanProduct.CustomerTypeId = x.CustomerTypeId;
            loanProduct.IsActive = x.IsActive;

            var result = _unitOfWork
                .GetWriteRepository<LoanProduct>()
                .Update(loanProduct);

            if (!result)
                return false;

            await _unitOfWork.SaveAsync();

            return true;
        }

        // ADMIN - DELETE

        public async Task<bool> DeleteAsync(Guid id)
        {
            var loanProduct =
                await _unitOfWork
                    .GetReadRepository<LoanProduct>()
                    .GetSingleAsync(x => x.Id == id);

            if (loanProduct == null)
                return false;

            loanProduct.IsActive = false;

            var result = _unitOfWork
                .GetWriteRepository<LoanProduct>()
                .Update(loanProduct);

            if (!result)
                return false;

            await _unitOfWork.SaveAsync();

            return true;
        }
        public async Task<List<ProductSearchResultDto>> SearchAsync(ProductSearchRequestDto request)
        {
            var query = _unitOfWork
                .GetReadRepository<LoanProduct>()
                .GetWhere(
                    x =>
                        x.IsActive &&
                        x.LoanTypeId == request.LoanTypeId &&
                        x.CustomerTypeId == request.CustomerTypeId &&
                        x.MinAmount <= request.Amount &&
                        x.MaxAmount >= request.Amount &&
                        x.MinTerm <= request.Term &&
                        x.MaxTerm >= request.Term,
                    false);

            if (request.BankIds != null && request.BankIds.Count > 0)
            {
                query = query.Where(x => request.BankIds.Contains(x.BankId));
            }

            var products = await query
                .Include(x => x.Bank)
                .Include(x => x.LoanType)
                .ToListAsync();

            var results = new List<ProductSearchResultDto>();

            foreach (var product in products)
            {
                decimal monthlyInterestRate = product.InterestRate / 100;
                decimal monthlyInstallment;

                if (monthlyInterestRate == 0)
                {
                    monthlyInstallment = request.Amount / request.Term;
                }
                else
                {
                    monthlyInstallment =
                        request.Amount *
                        monthlyInterestRate *
                        (decimal)Math.Pow((double)(1 + monthlyInterestRate), request.Term)
                        /
                        ((decimal)Math.Pow((double)(1 + monthlyInterestRate), request.Term) - 1);
                }

                monthlyInstallment = Math.Round(monthlyInstallment, 2);
                decimal totalPayment = Math.Round(monthlyInstallment * request.Term, 2);

                results.Add(new ProductSearchResultDto
                {
                    LoanProductId = product.Id,
                    BankName = product.Bank.Name,
                    LoanTypeName = product.LoanType.Name,
                    Amount = request.Amount,
                    Term = request.Term,
                    InterestRate = product.InterestRate,
                    MonthlyInstallment = monthlyInstallment,
                    TotalPayment = totalPayment
                });
            }

            return results;
        }

    }
}
    
