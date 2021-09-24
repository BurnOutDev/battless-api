using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ConfigurationMiddleware.Extensions;
using System.Text.Json;
using Api.Middleware;
using Domain.Helpers;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.SignalR;

namespace ConfigurationMiddleware
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddCors()
                .AddSwagger()
                .AddContext(Configuration)
                .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
                .Configure<AppSettings>(Configuration.GetSection("AppSettings"))
                .AddDI()
                .AddControllers()
                .AddJsonOptions(conf =>
                {
                    conf.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    conf.JsonSerializerOptions.IgnoreNullValues = true;
                });

            services.AddSignalR();
            services.AddSingleton<IUserIdProvider, IdBasedUserIdProvider>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var allowedHosts = Configuration.GetSection("AllowedHosts").GetChildren().ToArray();
            app.UseCors(x => x.WithOrigins(allowedHosts.Select(x => x.Value).ToArray()).AllowCredentials().AllowAnyHeader().AllowAnyMethod());

            app.UseRouting();

            var cultureInfo = new CultureInfo("en-US");

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            app
                .UseRouting()
                .AddSwagger()
                .UseMiddleware<ErrorHandlerMiddleware>()
                .UseMiddleware<JwtMiddleware>()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapGet("/", async context =>
                    {
                        await context.Response.WriteAsync("Hello World!");
                    });
                });
        }
    }
}
