using internLoanProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Persistence.Contexts
{
    public class internLoanProjectAPIDbContext : DbContext
    {
        public internLoanProjectAPIDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerType> CustomerTypes { get; set; }
        public DbSet<LoanApplication> LoanApplications { get; set; }
        public DbSet<LoanCalculation> LoanCalculations { get; set; }
        public DbSet<LoanProduct> LoanProducts { get; set; }
        public DbSet<LoanType> LoanTypes { get; set; }
        public DbSet<PaymentPlan> PaymentPlans { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            // BANK: SEED DATA ile eklendi

            builder.Entity<Bank>()
                .HasIndex(x => x.Name)
                .IsUnique();

            builder.Entity<Bank>().HasData(
            new Bank
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Akbank"
            },
            new Bank
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Garanti BBVA"
            },
            new Bank
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "İş Bankası"
            }

            );
            // LOAN TYPE

            builder.Entity<LoanType>()
                .HasIndex(x => x.Name)
                .IsUnique();



            // CUSTOMER TYPE

            builder.Entity<CustomerType>()
                .HasIndex(x => x.Name)
                .IsUnique();


            // CUSTOMER

            builder.Entity<Customer>()
                .HasOne(x => x.CustomerType)
                .WithMany(x => x.Customers)
                .HasForeignKey(x => x.CustomerTypeId)
                .OnDelete(DeleteBehavior.Restrict);



            // LOAN PRODUCT

            builder.Entity<LoanProduct>()
                .Property(x => x.InterestRate)
                .HasPrecision(18, 2);

            builder.Entity<LoanProduct>()
                .Property(x => x.MinAmount)
                .HasPrecision(18, 2);

            builder.Entity<LoanProduct>()
                .Property(x => x.MaxAmount)
                .HasPrecision(18, 2);


            // LoanProduct -> Bank

            builder.Entity<LoanProduct>()
                .HasOne(x => x.Bank)
                .WithMany(x => x.LoanProducts)
                .HasForeignKey(x => x.BankId)
                .OnDelete(DeleteBehavior.Restrict);


            // LoanProduct -> LoanType

            builder.Entity<LoanProduct>()
                .HasOne(x => x.LoanType)
                .WithMany(x => x.LoanProducts)
                .HasForeignKey(x => x.LoanTypeId)
                .OnDelete(DeleteBehavior.Restrict);


            // LoanProduct -> CustomerType

            builder.Entity<LoanProduct>()
                .HasOne(x => x.CustomerType)
                .WithMany(x => x.LoanProducts)
                .HasForeignKey(x => x.CustomerTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // LOAN CALCULATION

            builder.Entity<LoanCalculation>()
                .Property(x => x.Amount)
                .HasPrecision(18, 2);

            builder.Entity<LoanCalculation>()
                .Property(x => x.InterestRate)
                .HasPrecision(18, 2);

            builder.Entity<LoanCalculation>()
                .Property(x => x.MonthlyInstallment)
                .HasPrecision(18, 2);

            builder.Entity<LoanCalculation>()
                .Property(x => x.TotalInterest)
                .HasPrecision(18, 2);

            builder.Entity<LoanCalculation>()
                .Property(x => x.TotalPayment)
                .HasPrecision(18, 2);


            // LoanCalculation -> LoanProduct

            builder.Entity<LoanCalculation>()
                .HasOne(x => x.LoanProduct)
                .WithMany(x => x.LoanCalculations)
                .HasForeignKey(x => x.LoanProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // PAYMENT PLAN

            builder.Entity<PaymentPlan>()
                .Property(x => x.InstallmentAmount)
                .HasPrecision(18, 2);

            builder.Entity<PaymentPlan>()
                .Property(x => x.PrincipalAmount)
                .HasPrecision(18, 2);

            builder.Entity<PaymentPlan>()
                .Property(x => x.InterestAmount)
                .HasPrecision(18, 2);

            builder.Entity<PaymentPlan>()
                .Property(x => x.RemainingPrincipal)
                .HasPrecision(18, 2);


            // PaymentPlan -> LoanCalculation

            builder.Entity<PaymentPlan>()
                .HasOne(x => x.LoanCalculation)
                .WithMany(x => x.PaymentPlans)
                .HasForeignKey(x => x.LoanCalculationId)
                .OnDelete(DeleteBehavior.Cascade);

            // LOAN APPLICATION

            // LoanApplication -> Customer

            builder.Entity<LoanApplication>()
                .HasOne(x => x.Customer)
                .WithMany(x => x.LoanApplications)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);


            // LoanApplication -> LoanProduct

            builder.Entity<LoanApplication>()
                .HasOne(x => x.LoanProduct)
                .WithMany(x => x.LoanApplications)
                .HasForeignKey(x => x.LoanProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // LoanApplication -> LoanCalculation
            // 1 LoanCalculation = 1 LoanApplication

            builder.Entity<LoanApplication>()
                .HasOne(x => x.LoanCalculation)
                .WithOne(x => x.LoanApplication)
                .HasForeignKey<LoanApplication>(x => x.LoanCalculationId)
                .OnDelete(DeleteBehavior.Restrict);

             // =========================
            // LOAN TYPE
            // =========================

            var loanType1Id =
                Guid.Parse("44444444-4444-4444-4444-444444444444");

            var loanType2Id =
                Guid.Parse("55555555-5555-5555-5555-555555555555");

            builder.Entity<LoanType>().HasData(
                new LoanType
                {
                    Id = loanType1Id,
                    Name = "Taksitli Kredi"
                },
                new LoanType
                {
                    Id = loanType2Id,
                    Name = "Taksitli Ek Hesap"
                }
            );


            // =========================
            // CUSTOMER TYPE
            // =========================

            var customerType1Id =
                Guid.Parse("66666666-6666-6666-6666-666666666666");

            var customerType2Id =
                Guid.Parse("77777777-7777-7777-7777-777777777777");

            builder.Entity<CustomerType>().HasData(
                new CustomerType
                {
                    Id = customerType1Id,
                    Name = "Öğrenci"
                },
                new CustomerType
                {
                    Id = customerType2Id,
                    Name = "Esnaf"
                }
            );


            // =========================
            // LOAN PRODUCT
            // =========================

            builder.Entity<LoanProduct>().HasData(

                new LoanProduct
                {
                    Id = Guid.Parse(
                        "88888888-8888-8888-8888-888888888888"),

                    Name = "Taksitli Ek Hesap Öğrenci",

                    InterestRate = 3.25m,

                    MinAmount = 1000m,
                    MaxAmount = 50000m,

                    MinTerm = 3,
                    MaxTerm = 24,

                    IsActive = true,

                    // Akbank
                    BankId = Guid.Parse(
                        "11111111-1111-1111-1111-111111111111"),

                    // Taksitli Ek Hesap
                    LoanTypeId = loanType2Id,

                    // Öğrenci
                    CustomerTypeId = customerType1Id
                },

                new LoanProduct
                {
                    Id = Guid.Parse(
                        "99999999-9999-9999-9999-999999999999"),

                    Name = "Taksitli Kredi Esnaf",

                    InterestRate = 2.95m,

                    MinAmount = 5000m,
                    MaxAmount = 250000m,

                    MinTerm = 3,
                    MaxTerm = 12,

                    IsActive = true,

                    // Garanti BBVA
                    BankId = Guid.Parse(
                        "22222222-2222-2222-2222-222222222222"),

                    // Taksitli Kredi
                    LoanTypeId = loanType1Id,

                    // Esnaf
                    CustomerTypeId = customerType2Id
                },

                new LoanProduct
                {
                    Id = Guid.Parse(
                        "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),

                    Name = "Taksitli Kredi Öğrenci",

                    InterestRate = 3.10m,

                    MinAmount = 2000m,
                    MaxAmount = 75000m,

                    MinTerm = 3,
                    MaxTerm = 24,

                    IsActive = true,

                    // İş Bankası
                    BankId = Guid.Parse(
                        "33333333-3333-3333-3333-333333333333"),

                    // Taksitli Kredi
                    LoanTypeId = loanType1Id,

                    // Öğrenci
                    CustomerTypeId = customerType1Id
                }
            );

           
        }
    }

}   




