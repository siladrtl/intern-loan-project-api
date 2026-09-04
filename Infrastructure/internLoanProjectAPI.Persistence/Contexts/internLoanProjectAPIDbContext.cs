using internLoanProject.Domain.Entities;
using internLoanProject.Domain.Entities.Enums;
using internLoanProject.Domain.Entities.Identity;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace internLoanProjectAPI.Persistence.Contexts
{
    public class internLoanProjectAPIDbContext
        : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public internLoanProjectAPIDbContext(
            DbContextOptions<internLoanProjectAPIDbContext> options)
            : base(options)
        {
        }

        public DbSet<Bank> Banks { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<LoanApplication> LoanApplications { get; set; }

        public DbSet<LoanCalculation> LoanCalculations { get; set; }

        public DbSet<LoanProduct> LoanProducts { get; set; }

        public DbSet<LoanType> LoanTypes { get; set; }

        public DbSet<PaymentPlan> PaymentPlans { get; set; }

        public DbSet<CustomerVerificationDocument> VerificationDocuments {get; set;} 


        protected override void OnModelCreating(
            ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .HasOne(x => x.Customer)
                .WithOne()
                .HasForeignKey<AppUser>(x => x.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Bank>()
                .HasIndex(x => x.Name)
                .IsUnique();


            builder.Entity<Customer>()
                .HasIndex(x => x.NationalId)
                .IsUnique();

            builder.Entity<Customer>()
                .HasIndex(x => x.Email)
                .IsUnique();

            builder.Entity<Customer>()
                .Property(x => x.CustomerType)
                .HasConversion<int>();

            builder.Entity<CustomerVerificationDocument>()
                .HasOne(x => x.Customer)
                .WithOne(x => x.VerificationDocument)
                .HasForeignKey<CustomerVerificationDocument>(
                 x => x.CustomerId)
                 .OnDelete(DeleteBehavior.Cascade);
           
            builder.Entity<CustomerVerificationDocument>()
            .Property(x => x.Status)
            .HasConversion<int>();


            builder.Entity<LoanType>()
                .HasIndex(x => x.Name)
                .IsUnique();

            builder.Entity<LoanType>()
                .Property(x => x.KkdfRate)
                .HasPrecision(18, 2);

            builder.Entity<LoanType>()
                .Property(x => x.BsmvRate)
                .HasPrecision(18, 2);

            builder.Entity<LoanProduct>()
                .Property(x => x.InterestRate)
                .HasPrecision(18, 2);

            builder.Entity<LoanProduct>()
                .Property(x => x.MinAmount)
                .HasPrecision(18, 2);

            builder.Entity<LoanProduct>()
                .Property(x => x.MaxAmount)
                .HasPrecision(18, 2);


            builder.Entity<LoanProduct>()
                .Property(x => x.CustomerType)
                .HasConversion<int>();

            builder.Entity<LoanProduct>()
                .HasOne(x => x.Bank)
                .WithMany(x => x.LoanProducts)
                .HasForeignKey(x => x.BankId)
                .OnDelete(DeleteBehavior.Restrict);
 
            builder.Entity<LoanProduct>()
                .HasOne(x => x.LoanType)
                .WithMany(x => x.LoanProducts)
                .HasForeignKey(x => x.LoanTypeId)
                .OnDelete(DeleteBehavior.Restrict);


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
                .Property(x => x.TotalKkdf)
                .HasPrecision(18, 2);

            builder.Entity<LoanCalculation>()
                .Property(x => x.TotalBsmv)
                .HasPrecision(18, 2);

            builder.Entity<LoanCalculation>()
                .Property(x => x.TotalPayment)
                .HasPrecision(18, 2);

            builder.Entity<LoanCalculation>()
                .HasOne(x => x.LoanProduct)
                .WithMany(x => x.LoanCalculations)
                .HasForeignKey(x => x.LoanProductId)
                .OnDelete(DeleteBehavior.Restrict);

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
                .Property(x => x.KkdfAmount)
                .HasPrecision(18, 2);

            builder.Entity<PaymentPlan>()
                .Property(x => x.BsmvAmount)
                .HasPrecision(18, 2);

            builder.Entity<PaymentPlan>()
                .Property(x => x.RemainingPrincipal)
                .HasPrecision(18, 2);


            builder.Entity<PaymentPlan>()
                .HasOne(x => x.LoanCalculation)
                .WithMany(x => x.PaymentPlans)
                .HasForeignKey(x => x.LoanCalculationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<LoanApplication>()
                .HasOne(x => x.Customer)
                .WithMany(x => x.LoanApplications)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LoanApplication>()
                .HasOne(x => x.LoanProduct)
                .WithMany(x => x.LoanApplications)
                .HasForeignKey(x => x.LoanProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LoanApplication>()
                .HasOne(x => x.LoanCalculation)
                .WithOne(x => x.LoanApplication)
                .HasForeignKey<LoanApplication>(
                    x => x.LoanCalculationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Bank>().HasData(

                new Bank
                {
                    Id = 1,
                    Name = "Ziraat Bankası"
                },

                new Bank
                {
                    Id = 2,
                    Name = "Halkbank"
                },

                new Bank
                {
                    Id = 3,
                    Name = "VakıfBank"
                },

                new Bank
                {
                    Id = 4,
                    Name = "İş Bankası"
                },

                new Bank
                {
                    Id = 5,
                    Name = "Garanti BBVA"
                },

                new Bank
                {
                    Id = 6,
                    Name = "Yapı Kredi"
                },

                new Bank
                {
                    Id = 7,
                    Name = "Akbank"
                },

                new Bank
                {
                    Id = 8,
                    Name = "QNB"
                },

                new Bank
                {
                    Id = 9,
                    Name = "DenizBank"
                },

                new Bank
                {
                    Id = 10,
                    Name = "TEB"
                }
            );


            // ==========================================
            // LOAN TYPE SEED
            // ==========================================

            builder.Entity<LoanType>().HasData(

                // Genel bireysel tüketici kredisi
                new LoanType
                {
                    Id = 1,

                    Name = "İhtiyaç Kredisi",

                    KkdfRate = 15m,

                    BsmvRate = 15m
                },


                // Öğrenciye yönelik tüketici kredisi
                new LoanType
                {
                    Id = 2,

                    Name = "Eğitim Kredisi",

                    KkdfRate = 15m,

                    BsmvRate = 15m
                },


                // Ticari/esnaf kredisi
                new LoanType
                {
                    Id = 3,

                    Name = "Esnaf Kredisi",

                    KkdfRate = 0m,

                    // Proje modellemesi için.
                    BsmvRate = 5m
                },


                // Emekliye özel ihtiyaç ürünü
                new LoanType
                {
                    Id = 4,

                    Name = "Emekli Kredisi",

                    KkdfRate = 15m,

                    BsmvRate = 15m
                }
            );


            // ==========================================
            // LOAN PRODUCT SEED
            // ==========================================
       

            builder.Entity<LoanProduct>().HasData(


                // ======================================
                // ÖĞRENCİ
                // ======================================

                new LoanProduct
                {
                    Id = 1,

                    Name = "Öğrenci İhtiyaç Kredisi",

                    InterestRate = 3.25m,

                    MinAmount = 5000m,
                    MaxAmount = 100000m,

                    MinTerm = 3,
                    MaxTerm = 24,

                    IsActive = true,

                    BankId = 1,

                    LoanTypeId = 1,

                    CustomerType = CustomerType.Ogrenci
                },


                new LoanProduct
                {
                    Id = 2,

                    Name = "Eğitim Destek Kredisi",

                    InterestRate = 3.10m,

                    MinAmount = 2000m,
                    MaxAmount = 150000m,

                    MinTerm = 3,
                    MaxTerm = 24,

                    IsActive = true,

                    BankId = 4,

                    LoanTypeId = 2,

                    CustomerType = CustomerType.Ogrenci
                },


                new LoanProduct
                {
                    Id = 3,

                    Name = "Öğrenci Eğitim Finansmanı",

                    InterestRate = 3.20m,

                    MinAmount = 5000m,
                    MaxAmount = 100000m,

                    MinTerm = 3,
                    MaxTerm = 18,

                    IsActive = true,

                    BankId = 7,

                    LoanTypeId = 2,

                    CustomerType = CustomerType.Ogrenci
                },


                new LoanProduct
                {
                    Id = 4,

                    Name = "Genç İhtiyaç Kredisi",

                    InterestRate = 3.35m,

                    MinAmount = 5000m,
                    MaxAmount = 75000m,

                    MinTerm = 3,
                    MaxTerm = 18,

                    IsActive = true,

                    BankId = 10,

                    LoanTypeId = 1,

                    CustomerType = CustomerType.Ogrenci
                },


                // ======================================
                // ESNAF
                // ======================================

                new LoanProduct
                {
                    Id = 5,

                    Name = "Esnaf Destek Kredisi",

                    InterestRate = 2.85m,

                    MinAmount = 25000m,
                    MaxAmount = 750000m,

                    MinTerm = 3,
                    MaxTerm = 36,

                    IsActive = true,

                    BankId = 2,

                    LoanTypeId = 3,

                    CustomerType = CustomerType.Esnaf
                },


                new LoanProduct
                {
                    Id = 6,

                    Name = "Esnaf İşletme Kredisi",

                    InterestRate = 2.95m,

                    MinAmount = 20000m,
                    MaxAmount = 500000m,

                    MinTerm = 3,
                    MaxTerm = 36,

                    IsActive = true,

                    BankId = 3,

                    LoanTypeId = 3,

                    CustomerType = CustomerType.Esnaf
                },


                new LoanProduct
                {
                    Id = 7,

                    Name = "Esnaf Nakit Destek",

                    InterestRate = 3.05m,

                    MinAmount = 25000m,
                    MaxAmount = 400000m,

                    MinTerm = 3,
                    MaxTerm = 24,

                    IsActive = true,

                    BankId = 8,

                    LoanTypeId = 3,

                    CustomerType = CustomerType.Esnaf
                },


                new LoanProduct
                {
                    Id = 8,

                    Name = "Esnaf İhtiyaç Kredisi",

                    InterestRate = 3.40m,

                    MinAmount = 10000m,
                    MaxAmount = 250000m,

                    MinTerm = 3,
                    MaxTerm = 24,

                    IsActive = true,

                    BankId = 5,

                    LoanTypeId = 1,

                    CustomerType = CustomerType.Esnaf
                },


                new LoanProduct
                {
                    Id = 9,

                    Name = "KOBİ Esnaf Destek",

                    InterestRate = 3.00m,

                    MinAmount = 25000m,
                    MaxAmount = 600000m,

                    MinTerm = 3,
                    MaxTerm = 36,

                    IsActive = true,

                    BankId = 6,

                    LoanTypeId = 3,

                    CustomerType = CustomerType.Esnaf
                },


                // ======================================
                // EMEKLİ
                // ======================================

                new LoanProduct
                {
                    Id = 10,

                    Name = "Emekli İhtiyaç Kredisi",

                    InterestRate = 2.95m,

                    MinAmount = 5000m,
                    MaxAmount = 250000m,

                    MinTerm = 3,
                    MaxTerm = 24,

                    IsActive = true,

                    BankId = 1,

                    LoanTypeId = 4,

                    CustomerType = CustomerType.Emekli
                },


                new LoanProduct
                {
                    Id = 11,

                    Name = "Emekliye Özel Kredi",

                    InterestRate = 3.00m,

                    MinAmount = 5000m,
                    MaxAmount = 200000m,

                    MinTerm = 3,
                    MaxTerm = 24,

                    IsActive = true,

                    BankId = 4,

                    LoanTypeId = 4,

                    CustomerType = CustomerType.Emekli
                },


                new LoanProduct
                {
                    Id = 12,

                    Name = "Emekli Destek Kredisi",

                    InterestRate = 2.90m,

                    MinAmount = 5000m,
                    MaxAmount = 250000m,

                    MinTerm = 3,
                    MaxTerm = 24,

                    IsActive = true,

                    BankId = 3,

                    LoanTypeId = 4,

                    CustomerType = CustomerType.Emekli
                },


                new LoanProduct
                {
                    Id = 13,

                    Name = "Emekli İhtiyaç Finansmanı",

                    InterestRate = 3.25m,

                    MinAmount = 5000m,
                    MaxAmount = 150000m,

                    MinTerm = 3,
                    MaxTerm = 18,

                    IsActive = true,

                    BankId = 9,

                    LoanTypeId = 1,

                    CustomerType = CustomerType.Emekli
                }
            );
        }
    }
}