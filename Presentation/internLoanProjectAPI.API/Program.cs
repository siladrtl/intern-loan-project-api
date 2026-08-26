using FluentValidation;

using internLoanProject.Domain.Entities.Identity;

using internLoanProjectAPI.Application.Validators.Auth;

using internLoanProjectAPI.Persistence;
using internLoanProjectAPI.Persistence.Contexts;
using internLoanProjectAPI.Persistence.Seed;

using internLoanProjectAPI.SignalR;
using internLoanProjectAPI.SignalR.Hubs;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using System.Text;


var builder =
    WebApplication.CreateBuilder(args);



// CONTROLLERS


builder.Services.AddControllers();


// FLUENT VALIDATION


builder.Services
    .AddValidatorsFromAssemblyContaining<
        RegisterRequestDtoValidator
    >();



// HTTP CONTEXT


builder.Services.AddHttpContextAccessor();


// SIGNALR


builder.Services.AddSignalR();


// CORS


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
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});



// SWAGGER


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();



// PERSISTENCE


builder.Services.AddPersistenceServices();



// SIGNALR SERVICES


builder.Services.AddSignalRServices();



// IDENTITY


builder.Services
    .AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<
        internLoanProjectAPIDbContext
    >()
    .AddDefaultTokenProviders();



// JWT KEY


var jwtKey =  builder.Configuration["Jwt:Key"]
    ?? throw new Exception("Jwt:Key appsettings.json içerisinde bulunamadý.");



// JWT AUTHENTICATION


builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = false,

                ValidateAudience = false,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };


     
        // SIGNALR JWT TOKEN
        

        options.Events =
            new JwtBearerEvents
            {
                OnMessageReceived =
                    context =>
                    {
                        var accessToken =
                            context.Request
                                .Query["access_token"];


                        var path =
                            context.HttpContext
                                .Request.Path;


                        if (
                            !string.IsNullOrEmpty(
                                accessToken
                            )
                            &&
                            path.StartsWithSegments(
                                "/notificationHub"
                            )
                        )
                        {
                            context.Token =
                                accessToken;
                        }


                        return Task.CompletedTask;
                    }
            };
    });


// APPLICATION BUILD


var app = builder.Build();


// IDENTITY SEED


using (var scope =
       app.Services.CreateScope())
{
    await IdentitySeeder
        .SeedRolesAsync(
            scope.ServiceProvider
        );


    await IdentitySeeder
        .SeedAdminAsync(
            scope.ServiceProvider
        );
}


// DEVELOPMENT


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}


// MIDDLEWARE


app.UseHttpsRedirection();

app.UseCors(
    "AllowAngularApp"
);

app.UseAuthentication();

app.UseAuthorization();


// CONTROLLERS

app.MapControllers();


// SIGNALR HUB


app.MapHub<NotificationHub>( "/notificationHub");


// RUN


app.Run();