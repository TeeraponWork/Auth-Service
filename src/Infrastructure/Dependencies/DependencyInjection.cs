using Domain.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Dependencies
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg, bool useMockRepo = false)
        {
            services.AddDbContext<AuthDbContext>(opt =>
                opt.UseNpgsql(cfg.GetConnectionString("AuthDb"))
                   .UseSnakeCaseNamingConvention()
            );

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            return services;
        }
    }
}
