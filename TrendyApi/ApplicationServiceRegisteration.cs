using Application.Contracts;
using Application.Features.Authentication;
using Application.Features.Authentication.Dtos.Mapping;
using Persistence.Repositories;

namespace TrendyApi
{
    public static class ApplicationServiceRegisteration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(AuthenticationMapping).Assembly));
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}