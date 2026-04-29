using Application.Common.Models;
using Application.Contracts;
using Application.Features.Authentication;
using Application.Features.Authentication.Dtos.Mapping;
using Application.Features.Authentication.Dtos.Validators;
using FluentValidation;
using Persistence.Repositories;

namespace TrendyApi
{
    public static class ApplicationServiceRegisteration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services ,IConfiguration configuration)
        {
            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(AuthenticationMapping).Assembly));
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IEmailService, EmailService>();
            services.Configure<EmailSettings>(
            configuration.GetSection("EmailSettings"));

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register validators
            services.AddValidatorsFromAssembly(typeof(RegisterRequestValidator).Assembly);

            return services;
        }
    }
}