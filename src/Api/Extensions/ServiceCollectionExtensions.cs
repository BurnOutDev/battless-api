using Application;
using DinkToPdf;
using DinkToPdf.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Persistence;
using System;
using System.IO;

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
            return services;
        }

        public static IServiceCollection AddDI(this IServiceCollection services)
        {
            var libwkhtmlFileName = "libwkhtmltox";

            if (OperatingSystem.IsLinux()) {
                libwkhtmlFileName = "libwkhtmltox.so";
            } else if (OperatingSystem.IsWindows()) {
                libwkhtmlFileName = "libwkhtmltox.dll";
            } else if (OperatingSystem.IsMacOS()) {
                libwkhtmlFileName = "libwkhtmltox.dylib";
            }
            
            var context = new CustomAssemblyLoadContext();
            context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), libwkhtmlFileName));

            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<ICourseService, CourseService>();
            services.AddSingleton<IEmailService, EmailService>();

            services.AddSingleton<MongoDbRepository<Account>>();
            services.AddSingleton<MongoDbRepository<Course>>();

            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            return services;
        }
    }
}
