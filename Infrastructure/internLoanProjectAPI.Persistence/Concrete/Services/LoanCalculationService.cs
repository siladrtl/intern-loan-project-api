using internLoanProject.Domain.Entities;
using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.Abstractions.UnitOfWorks;
using internLoanProjectAPI.Application.DTOs.Calculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Persistence.Concrete.Services
{
    public class LoanCalculationService : ILoanCalculationService
    {
        
            private readonly IUnitOfWork _unitOfWork;

            public LoanCalculationService(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<LoanCalculationDto> CalculateAsync(
                CreateLoanCalculationDto dto)
            {
                var loanProduct = await _unitOfWork
                    .GetReadRepository<LoanProduct>()
                    .GetSingleAsync(
                        x => x.Id == dto.LoanProductId &&
                             x.IsActive,
                        false);

                if (loanProduct == null)
                    throw new Exception("Kredi ürünü bulunamadı");

                // Tutar kontrolü
                if (dto.Amount < loanProduct.MinAmount ||
                    dto.Amount > loanProduct.MaxAmount)
                {
                    throw new Exception(
                        "Kredi tutarı verilen aralığın dışında.");
                }

                // Vade kontrolü
                if (dto.Term < loanProduct.MinTerm ||
                    dto.Term > loanProduct.MaxTerm)
                {
                    throw new Exception(
                        "Kredi vadesi verilen aralığın dışında. Farklı bir kredi tutarı giriniz.");
                }

                decimal monthlyInterestRate =
                    loanProduct.InterestRate / 100;

                decimal monthlyInstallment;

                if (monthlyInterestRate == 0)
                {
                    monthlyInstallment =
                        dto.Amount / dto.Term;
                }
                else
                {
                    monthlyInstallment =
                        dto.Amount *
                        monthlyInterestRate *
                        (decimal)Math.Pow(
                            (double)(1 + monthlyInterestRate),
                            dto.Term)
                        /
                        ((decimal)Math.Pow(
                            (double)(1 + monthlyInterestRate),
                            dto.Term) - 1);
                }

                monthlyInstallment =
                    Math.Round(monthlyInstallment, 2);

                decimal totalPayment =
                    Math.Round(
                        monthlyInstallment * dto.Term,
                        2);

                decimal totalInterest =
                    totalPayment - dto.Amount;

                // LoanCalculation oluştur
                var calculation = new LoanCalculation
                {
                    Id = Guid.NewGuid(),

                    LoanProductId = loanProduct.Id,

                    Amount = dto.Amount,
                    Term = dto.Term,

                    InterestRate = loanProduct.InterestRate,

                    MonthlyInstallment = monthlyInstallment,

                    TotalPayment = totalPayment,

                    TotalInterest = totalInterest,

                    PaymentPlans = new List<PaymentPlan>()
                };

                // Ödeme planı
                decimal remainingPrincipal = dto.Amount;

                for (int i = 1; i <= dto.Term; i++)
                {
                    decimal interestAmount =
                        Math.Round(
                            remainingPrincipal *
                            monthlyInterestRate,
                            2);

                    decimal principalAmount =
                        monthlyInstallment -
                        interestAmount;

                    principalAmount =
                        Math.Round(principalAmount, 2);

                    remainingPrincipal -= principalAmount;

                    if (i == dto.Term)
                        remainingPrincipal = 0;

                    calculation.PaymentPlans.Add(
                        new PaymentPlan
                        {
                            Id = Guid.NewGuid(),

                            InstallmentNumber = i,

                            DueDate = DateTime.UtcNow
                                .AddMonths(i),

                            InstallmentAmount =
                                monthlyInstallment,

                            PrincipalAmount =
                                principalAmount,

                            InterestAmount =
                                interestAmount,

                            RemainingPrincipal =
                                Math.Round(
                                    remainingPrincipal,
                                    2)
                        });
                }

                await _unitOfWork
                    .GetWriteRepository<LoanCalculation>()
                    .AddAsync(calculation);

                await _unitOfWork.SaveAsync();

                return new LoanCalculationDto
                {
                    Id = calculation.Id,

                    LoanProductId = loanProduct.Id,

                    LoanProductName = loanProduct.Name,

                    Amount = calculation.Amount,

                    Term = calculation.Term,

                    InterestRate = calculation.InterestRate,

                    MonthlyInstallment =calculation.MonthlyInstallment,

                    TotalInterest = calculation.TotalInterest,

                    TotalPayment = calculation.TotalPayment,

                    PaymentPlans = calculation.PaymentPlans
                            .Select(x => new PaymentPlanDto
                            {
                                InstallmentNumber = x.InstallmentNumber,

                                DueDate = x.DueDate,

                                InstallmentAmount = x.InstallmentAmount,

                                PrincipalAmount = x.PrincipalAmount,

                                InterestAmount = x.InterestAmount,

                                RemainingPrincipal =  x.RemainingPrincipal
                            })
                            .ToList()
                };
            }
        }
    }

