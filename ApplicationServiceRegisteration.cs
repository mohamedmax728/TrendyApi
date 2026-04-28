using Application.Common.Models;
using Application.Contracts;
using Application.Features.Authentication;
using Application.Features.Authentication.Dtos.Mapping;
using Application.Features.Authentication.Dtos.Validators;
using Application.Features.Products.Services;
using FluentValidation;
using Persistence.Repositories;
using System.Reflection;

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
            
            // Register Product services - repositories are resolved via UnitOfWork lazy initialization
            services.AddScoped<IProductService, ProductService>();

            // Register validators
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
