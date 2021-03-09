using Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Persistence;

namespace ConfigurationMiddleware.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Client Branding API",
                    Version = "v1"
                });
            });

            return services;
        }

        public static IServiceCollection AddContext(this IServiceCollection services, IConfiguration configuration)
        {
            var i = configuration.GetSection("AppSettings").GetSection("UseInMemory").Get<bool>();

            if (i)
            {
                services.AddDbContext<GamblingDbContext>(x =>
                {
                    x.UseInMemoryDatabase("InMemoryDatabase");
                }, ServiceLifetime.Singleton);
            }
            else
            {
                services.AddDbContext<GamblingDbContext>(x =>
                {
                    x.UseSqlServer(configuration.GetConnectionString("Connection"));
                }, ServiceLifetime.Singleton);
            }
          
            return services;
        }

        public static IServiceCollection AddDI(this IServiceCollection services)
        {
            services.AddSingleton<IClientConfigurationService, ClientConfigurationService>();

            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<IEmailService, EmailService>();

            return services;
        }
    }
}
