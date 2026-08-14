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


            // BANK

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
        }
    }

}   




