using internLoanProject.Domain.Entities;
using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.Abstractions.UnitOfWorks;
using internLoanProjectAPI.Application.DTOs.Calculation;

namespace internLoanProjectAPI.Persistence.Concrete.Services
{
    public class LoanCalculationService : ILoanCalculationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LoanCalculationService(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<LoanCalculationDto> CalculateAsync(
            CreateLoanCalculationDto dto)
        {
       
            var loanProduct = await _unitOfWork
                .GetReadRepository<LoanProduct>()
                .GetSingleAsync(
                    x =>
                        x.Id == dto.LoanProductId &&
                        x.IsActive,
                    false);


            if (loanProduct == null)
            {
                throw new Exception(
                    "Kredi ürünü bulunamadı.");
            }

            var loanType = await _unitOfWork
                .GetReadRepository<LoanType>()
                .GetSingleAsync(
                    x => x.Id == loanProduct.LoanTypeId,
                    false);


            if (loanType == null)
            {
                throw new Exception(
                    "Kredi türü bulunamadı.");
            }


            if (dto.Amount < loanProduct.MinAmount ||
                dto.Amount > loanProduct.MaxAmount)
            {
                throw new Exception(
                    "Kredi tutarı ürün için geçerli aralığın dışındadır.");
            }


            if (dto.Term < loanProduct.MinTerm ||
                dto.Term > loanProduct.MaxTerm)
            {
                throw new Exception(
                    "Kredi vadesi ürün için geçerli aralığın dışındadır.");
            }
            decimal monthlyInterestRate =
                loanProduct.InterestRate / 100m;

            decimal kkdfRate =
                loanType.KkdfRate / 100m;

            decimal bsmvRate =
                loanType.BsmvRate / 100m;

            decimal effectiveMonthlyRate =
                monthlyInterestRate *
                (1 + kkdfRate + bsmvRate);


            decimal monthlyInstallment;


            if (effectiveMonthlyRate == 0)
            {
                monthlyInstallment =
                    dto.Amount / dto.Term;
            }
            else
            {
                decimal factor =
                    (decimal)Math.Pow(
                        (double)(1 + effectiveMonthlyRate),
                        dto.Term);


                monthlyInstallment =
                    dto.Amount *
                    effectiveMonthlyRate *
                    factor
                    /
                    (factor - 1);
            }


            monthlyInstallment =
                Math.Round(
                    monthlyInstallment,
                    2);

            var calculation =
                new LoanCalculation
                {
    
                    LoanProductId =
                        loanProduct.Id,

                    Amount =
                        dto.Amount,

                    Term =
                        dto.Term,

                    InterestRate =
                        loanProduct.InterestRate,

                    MonthlyInstallment =
                        monthlyInstallment,

                    PaymentPlans =
                        new List<PaymentPlan>()
                };

            decimal remainingPrincipal =
                dto.Amount;

            decimal totalInterest = 0;

            decimal totalKkdf = 0;

            decimal totalBsmv = 0;

            decimal totalPayment = 0;


            for (int i = 1; i <= dto.Term; i++)
            {
   
                decimal interestAmount =
                    Math.Round(
                        remainingPrincipal *
                        monthlyInterestRate,
                        2);


                decimal kkdfAmount =
                    Math.Round(
                        interestAmount *
                        kkdfRate,
                        2);

                decimal bsmvAmount =
                    Math.Round(
                        interestAmount *
                        bsmvRate,
                        2);


                decimal principalAmount =
                    monthlyInstallment
                    - interestAmount
                    - kkdfAmount
                    - bsmvAmount;


                principalAmount =
                    Math.Round(
                        principalAmount,
                        2);

                if (i == dto.Term)
                {
                    principalAmount =
                        remainingPrincipal;

                    monthlyInstallment =
                        Math.Round(
                            principalAmount
                            + interestAmount
                            + kkdfAmount
                            + bsmvAmount,
                            2);
                }


                remainingPrincipal -=
                    principalAmount;


                if (remainingPrincipal < 0 ||
                    i == dto.Term)
                {
                    remainingPrincipal = 0;
                }

                totalInterest +=
                    interestAmount;

                totalKkdf +=
                    kkdfAmount;

                totalBsmv +=
                    bsmvAmount;

                totalPayment +=
                    monthlyInstallment;


                calculation.PaymentPlans.Add(
                    new PaymentPlan
                    {
                  
                      
                        InstallmentNumber =
                            i,

                        DueDate =
                            DateTime.Now.AddMonths(i),

                        InstallmentAmount =
                            monthlyInstallment,

                        PrincipalAmount =
                            principalAmount,

                        InterestAmount =
                            interestAmount,

                        KkdfAmount =
                            kkdfAmount,

                        BsmvAmount =
                            bsmvAmount,

                        RemainingPrincipal =
                            Math.Round(
                                remainingPrincipal,
                                2)
                    });
            }


            // ==========================================
            // TOPLAMLARI CALCULATION'A YAZ
            // ==========================================

            calculation.TotalInterest =
                Math.Round(
                    totalInterest,
                    2);

            calculation.TotalKkdf =
                Math.Round(
                    totalKkdf,
                    2);

            calculation.TotalBsmv =
                Math.Round(
                    totalBsmv,
                    2);

            calculation.TotalPayment =
                Math.Round(
                    totalPayment,
                    2);


            await _unitOfWork
                .GetWriteRepository<LoanCalculation>()
                .AddAsync(calculation);


            await _unitOfWork.SaveAsync();

            return new LoanCalculationDto
            {
                Id =
                    calculation.Id,

                LoanProductId =
                    loanProduct.Id,

                LoanProductName =
                    loanProduct.Name,

                Amount =
                    calculation.Amount,

                Term =
                    calculation.Term,

                InterestRate =
                    calculation.InterestRate,

                MonthlyInstallment =
                    calculation.MonthlyInstallment,

                TotalInterest =
                    calculation.TotalInterest,

                TotalKkdf =
                    calculation.TotalKkdf,

                TotalBsmv =
                    calculation.TotalBsmv,

                TotalPayment =
                    calculation.TotalPayment,

                PaymentPlans =
                    calculation.PaymentPlans
                        .Select(x =>
                            new PaymentPlanDto
                            {
                                InstallmentNumber =
                                    x.InstallmentNumber,

                                DueDate =
                                    x.DueDate,

                                InstallmentAmount =
                                    x.InstallmentAmount,

                                PrincipalAmount =
                                    x.PrincipalAmount,

                                InterestAmount =
                                    x.InterestAmount,

                                KkdfAmount =
                                    x.KkdfAmount,

                                BsmvAmount =
                                    x.BsmvAmount,

                                RemainingPrincipal =
                                    x.RemainingPrincipal
                            })
                        .ToList()
            };
        }
    }
}