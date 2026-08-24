using internLoanProject.Domain.Entities;
using internLoanProject.Domain.Entities.Enums;
using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.Abstractions.UnitOfWorks;
using internLoanProjectAPI.Application.DTOs.Product;

using Microsoft.EntityFrameworkCore;

namespace internLoanProjectAPI.Persistence.Concrete.Services
{
    public class LoanProductService : ILoanProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LoanProductService(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        // ==========================================
        // TÜM AKTİF ÜRÜNLER
        // ==========================================

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

                    LoanTypeName = x.LoanType.Name,

                    CustomerType = x.CustomerType,

                    IsActive = x.IsActive
                })
                .ToListAsync();
        }


        // ==========================================
        // KREDİ TÜRÜ + MÜŞTERİ TİPİ
        // ==========================================

        public async Task<List<LoanProductDto>>
            GetByLoanTypeAsync(
                int loanTypeId,
                CustomerType customerType)
        {
            return await _unitOfWork
                .GetReadRepository<LoanProduct>()
                .GetWhere(
                    x =>
                        x.LoanTypeId == loanTypeId &&
                        x.CustomerType == customerType &&
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

                    LoanTypeName = x.LoanType.Name,

                    CustomerType = x.CustomerType,

                    IsActive = x.IsActive
                })
                .ToListAsync();
        }


        // ==========================================
        // ADMIN - ADD
        // ==========================================

        public async Task<bool> AddAsync(
            CreateLoanProductRequestDto dto)
        {
            var loanProduct = new LoanProduct
            {
                // Id vermiyoruz.
                // Database otomatik int ID üretecek.

                Name = dto.Name,

                InterestRate = dto.InterestRate,

                MinAmount = dto.MinAmount,

                MaxAmount = dto.MaxAmount,

                MinTerm = dto.MinTerm,

                MaxTerm = dto.MaxTerm,

                BankId = dto.BankId,

                LoanTypeId = dto.LoanTypeId,

                CustomerType = dto.CustomerType,

                IsActive = true
            };


            var result = await _unitOfWork
                .GetWriteRepository<LoanProduct>()
                .AddAsync(loanProduct);


            if (!result)
            {
                return false;
            }


            await _unitOfWork.SaveAsync();

            return true;
        }


        // ==========================================
        // ADMIN - UPDATE
        // ==========================================

        public async Task<bool> UpdateAsync(
            int id,
            UpdateLoanProductRequestDto dto)
        {
            var loanProduct =
                await _unitOfWork
                    .GetReadRepository<LoanProduct>()
                    .GetSingleAsync(
                        x => x.Id == id);


            if (loanProduct == null)
            {
                return false;
            }


            loanProduct.Name =
                dto.Name;

            loanProduct.InterestRate =
                dto.InterestRate;

            loanProduct.MinAmount =
                dto.MinAmount;

            loanProduct.MaxAmount =
                dto.MaxAmount;

            loanProduct.MinTerm =
                dto.MinTerm;

            loanProduct.MaxTerm =
                dto.MaxTerm;

            loanProduct.BankId =
                dto.BankId;

            loanProduct.LoanTypeId =
                dto.LoanTypeId;

            loanProduct.CustomerType =
                dto.CustomerType;

            loanProduct.IsActive =
                dto.IsActive;


            var result =
                _unitOfWork
                    .GetWriteRepository<LoanProduct>()
                    .Update(loanProduct);


            if (!result)
            {
                return false;
            }


            await _unitOfWork.SaveAsync();

            return true;
        }


        // ==========================================
        // ADMIN - SOFT DELETE
        // ==========================================

        public async Task<bool> DeleteAsync(
            int id)
        {
            var loanProduct =
                await _unitOfWork
                    .GetReadRepository<LoanProduct>()
                    .GetSingleAsync(
                        x => x.Id == id);


            if (loanProduct == null)
            {
                return false;
            }


            loanProduct.IsActive = false;


            var result =
                _unitOfWork
                    .GetWriteRepository<LoanProduct>()
                    .Update(loanProduct);


            if (!result)
            {
                return false;
            }


            await _unitOfWork.SaveAsync();

            return true;
        }


        // ==========================================
        // ÜRÜN ARAMA
        // ==========================================

        public async Task<List<ProductSearchResultDto>>
            SearchAsync(
                ProductSearchRequestDto request)
        {
            // ------------------------------------------
            // TEMEL FİLTRE
            // ------------------------------------------

            var query = _unitOfWork
                .GetReadRepository<LoanProduct>()
                .GetWhere(
                    x =>
                        x.IsActive &&

                        x.LoanTypeId ==
                        request.LoanTypeId &&

                        x.MinAmount <=
                        request.Amount &&

                        x.MaxAmount >=
                        request.Amount &&

                        x.MinTerm <=
                        request.Term &&

                        x.MaxTerm >=
                        request.Term,
                    false);


            // ------------------------------------------
            // MÜŞTERİ TİPİ OPSİYONEL
            // ------------------------------------------

            if (request.CustomerType.HasValue)
            {
                query = query.Where(
                    x =>
                        x.CustomerType ==
                        request.CustomerType.Value);
            }


            // ------------------------------------------
            // BANKA FİLTRESİ OPSİYONEL
            // ------------------------------------------

            if (request.BankIds != null &&
                request.BankIds.Count > 0)
            {
                query = query.Where(
                    x =>
                        request.BankIds.Contains(
                            x.BankId));
            }


            // ------------------------------------------
            // İLİŞKİLERİ GETİR
            // ------------------------------------------

            var products =
                await query
                    .Include(x => x.Bank)
                    .Include(x => x.LoanType)
                    .ToListAsync();


            var results =
                new List<ProductSearchResultDto>();


            // ------------------------------------------
            // HER ÜRÜN İÇİN HESAPLAMA
            // ------------------------------------------

            foreach (var product in products)
            {
                decimal monthlyInterestRate =
                    product.InterestRate / 100m;


                decimal kkdfRate =
                    product.LoanType.KkdfRate / 100m;


                decimal bsmvRate =
                    product.LoanType.BsmvRate / 100m;


                // Faiz üzerine uygulanan KKDF ve BSMV
                // nedeniyle efektif aylık oran

                decimal effectiveMonthlyRate =
                    monthlyInterestRate *
                    (1 + kkdfRate + bsmvRate);


                decimal monthlyInstallment;


                if (effectiveMonthlyRate == 0)
                {
                    monthlyInstallment =
                        request.Amount /
                        request.Term;
                }
                else
                {
                    decimal factor =
                        (decimal)Math.Pow(
                            (double)
                            (1 + effectiveMonthlyRate),
                            request.Term);


                    monthlyInstallment =
                        request.Amount *
                        effectiveMonthlyRate *
                        factor
                        /
                        (factor - 1);
                }


                monthlyInstallment =
                    Math.Round(
                        monthlyInstallment,
                        2);


                decimal totalPayment =
                    Math.Round(
                        monthlyInstallment *
                        request.Term,
                        2);


                // --------------------------------------
                // RESPONSE
                // --------------------------------------

                results.Add(
                    new ProductSearchResultDto
                    {
                        LoanProductId =
                            product.Id,

                        BankId = product.BankId,

                        LoanProductName =
                            product.Name,

                        BankName =
                            product.Bank.Name,

                        LoanTypeName =
                            product.LoanType.Name,

                        CustomerType =
                            product.CustomerType,

                        Amount =
                            request.Amount,

                        Term =
                            request.Term,

                        InterestRate =
                            product.InterestRate,

                        MonthlyInstallment =
                            monthlyInstallment,

                        TotalPayment =
                            totalPayment
                    });
            }


            return results;
        }
    }
}