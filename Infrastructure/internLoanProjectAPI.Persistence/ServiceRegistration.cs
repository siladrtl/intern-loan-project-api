using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.Abstractions.UnitOfWorks;
using internLoanProjectAPI.Application.Repositories;
using internLoanProjectAPI.Persistence.Concrete.Services;
using internLoanProjectAPI.Persistence.Concrete.UnitOfWorks;
using internLoanProjectAPI.Persistence.Contexts;
using internLoanProjectAPI.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection services)
        {
            services.AddDbContext<internLoanProjectAPIDbContext>(options => options.UseSqlServer(Configuration.ConnectionString));
            
            //Unit Of Work DI Kaydi

            services.AddScoped<IUnitOfWork, UnitOfWork>();
       
           
            //Servis DI Kaydi

            services.AddScoped<IBankService, BankService>();
            services.AddScoped<ILoanTypeService, LoanTypeService>();
            services.AddScoped<ILoanProductService, LoanProductService>();
            services.AddScoped<ILoanCalculationService, LoanCalculationService>();   
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ILoanApplicationService, LoanApplicationService>();
            services.AddScoped<IAdminLoanApplicationService, AdminLoanApplicationService>();
        }
    }
}
