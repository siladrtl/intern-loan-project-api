using internLoanProject.Domain.Entities.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace internLoanProjectAPI.Persistence.Seed
{
    public static class IdentitySeeder
    {
        // ==========================================
        // ROLLERİ OLUŞTUR
        // ==========================================

        public static async Task SeedRolesAsync(
            IServiceProvider serviceProvider)
        {
            var roleManager =
                serviceProvider
                    .GetRequiredService<RoleManager<AppRole>>();


            string[] roles =
            {
                "Customer",
                "Admin"
            };


            foreach (var roleName in roles)
            {
                var roleExists =
                    await roleManager
                        .RoleExistsAsync(roleName);


                // Rol zaten varsa tekrar oluşturma
                if (roleExists)
                {
                    continue;
                }


                var result =
                    await roleManager
                        .CreateAsync(
                            new AppRole
                            {
                                Name = roleName
                            }
                        );


                if (!result.Succeeded)
                {
                    var errors =
                        string.Join(
                            ", ",
                            result.Errors
                                .Select(
                                    x => x.Description
                                )
                        );


                    throw new Exception(
                        $"{roleName} rolü oluşturulamadı: {errors}"
                    );
                }
            }
        }


        // ==========================================
        // ADMIN KULLANICISI OLUŞTUR
        // ==========================================

        public static async Task SeedAdminAsync(
            IServiceProvider serviceProvider)
        {
            var userManager =
                serviceProvider
                    .GetRequiredService<UserManager<AppUser>>();


            // Şimdilik test/admin hesabımız
            const string adminEmail =
                "admin@loanapp.com";

            const string adminPassword =
                "Admin123!";


            // ======================================
            // ADMIN ZATEN VAR MI?
            // ======================================

            var adminUser =
                await userManager
                    .FindByEmailAsync(
                        adminEmail
                    );


            // ======================================
            // YOKSA ADMIN OLUŞTUR
            // ======================================

            if (adminUser == null)
            {
                adminUser =
                    new AppUser
                    {
                        UserName =
                            adminEmail,

                        Email =
                            adminEmail,

                        EmailConfirmed =
                            true,

                        // Admin müşteri olmadığı için
                        // CustomerId null bırakıyoruz.
                        CustomerId =
                            null
                    };


                var createResult =
                    await userManager
                        .CreateAsync(
                            adminUser,
                            adminPassword
                        );


                if (!createResult.Succeeded)
                {
                    var errors =
                        string.Join(
                            ", ",
                            createResult.Errors
                                .Select(
                                    x => x.Description
                                )
                        );


                    throw new Exception(
                        $"Admin kullanıcısı oluşturulamadı: {errors}"
                    );
                }
            }


            // ======================================
            // ADMIN ROLÜ VAR MI?
            // ======================================

            var isAdmin =
                await userManager
                    .IsInRoleAsync(
                        adminUser,
                        "Admin"
                    );


            // ======================================
            // YOKSA ADMIN ROLÜ VER
            // ======================================

            if (!isAdmin)
            {
                var roleResult =
                    await userManager
                        .AddToRoleAsync(
                            adminUser,
                            "Admin"
                        );


                if (!roleResult.Succeeded)
                {
                    var errors =
                        string.Join(
                            ", ",
                            roleResult.Errors
                                .Select(
                                    x => x.Description
                                )
                        );


                    throw new Exception(
                        $"Admin rolü atanamadı: {errors}"
                    );
                }
            }
        }
    }
}