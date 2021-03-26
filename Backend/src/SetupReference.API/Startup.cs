using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using SetupReference.API;
using SetupReference.API.Middleware.Health;
using SetupReference.Database;
using SetupReference.Database.Example;

namespace SetupReference
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly bool isDevelopment;

        public Startup(IHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            this.configuration = configuration;
            isDevelopment = hostEnvironment.IsDevelopment();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddServiceLayerServices(configuration);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SetupReference", Version = "v1" });
            });

            services
                .AddHealthChecks()
                .AddCheck<LivenessHealthCheck>("liveness", tags: new[] { "liveness" });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<ITenantProvider<int>, WebTenantProvider>();

            if (!isDevelopment)
                services.AddDataProtectionWithAzure(configuration.GetSection("DataProtection"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SetupReference v1"));
            }

            // UseSerilogRequestLogging() requires Serilog.UseAspNetCore
            // This condenses the logging output for HTTP requests
            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseRouting();

            // Register health check middleware(s) before auth (makes /health requests public)
            app.UseHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = registration => registration.Tags.Contains("liveness")
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();

            var context = serviceScope.ServiceProvider.GetService<ExampleContext>();

            //var context = new ExampleContextFactory().CreateDbContext(new string[] { });
            // Reset the database on each run - clearly this is not production code
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Could call this instead to perform migrations - it is mutually exclusive to the above
            //context.Database.Migrate();
        }
    }
}
