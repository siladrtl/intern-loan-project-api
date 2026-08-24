using FluentValidation;

using internLoanProject.Domain.Entities.Identity;

using internLoanProjectAPI.Application.Validators.Auth;

using internLoanProjectAPI.Persistence;
using internLoanProjectAPI.Persistence.Contexts;
using internLoanProjectAPI.Persistence.Seed;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using System.Text;


var builder = WebApplication.CreateBuilder(args);


// ==========================================
// CONTROLLERS
// ==========================================

builder.Services.AddControllers();


// ==========================================
// FLUENT VALIDATION
// ==========================================

builder.Services
    .AddValidatorsFromAssemblyContaining<
        RegisterRequestDtoValidator
    >();


// ==========================================
// HTTP CONTEXT ACCESSOR
// ==========================================

builder.Services.AddHttpContextAccessor();


// ==========================================
// CORS
// ==========================================

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAngularApp",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:4200"
                )
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    );
});


// ==========================================
// SWAGGER
// ==========================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();


// ==========================================
// PERSISTENCE
// ==========================================

builder.Services.AddPersistenceServices();


// ==========================================
// IDENTITY
// ==========================================

builder.Services
    .AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<
        internLoanProjectAPIDbContext
    >()
    .AddDefaultTokenProviders();


// ==========================================
// JWT
// ==========================================

var jwtKey =
    builder.Configuration["Jwt:Key"];


if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new Exception(
        "Jwt:Key appsettings.json içerisinde bulunamadý."
    );
}


builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer =
                    false,

                ValidateAudience =
                    false,

                ValidateLifetime =
                    true,

                ValidateIssuerSigningKey =
                    true,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtKey
                        )
                    )
            };
    });


// ==========================================
// APPLICATION BUILD
// ==========================================

var app =
    builder.Build();


// ==========================================
// IDENTITY ROLE SEED
// ==========================================

using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedRolesAsync(
        scope.ServiceProvider
    );

    await IdentitySeeder.SeedAdminAsync(
        scope.ServiceProvider
    );
}

// ==========================================
// DEVELOPMENT
// ==========================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}


// ==========================================
// MIDDLEWARE
// ==========================================

app.UseHttpsRedirection();

app.UseCors(
    "AllowAngularApp"
);


// Önce kim olduðunu belirle
app.UseAuthentication();

// Sonra yetkisi var mý kontrol et
app.UseAuthorization();


// ==========================================
// CONTROLLERS
// ==========================================

app.MapControllers();


// ==========================================
// RUN
// ==========================================

app.Run();