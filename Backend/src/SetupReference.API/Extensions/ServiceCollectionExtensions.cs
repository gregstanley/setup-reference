using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MediatR;
using SetupReference;
using SetupReference.Database.Example;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServiceLayerServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            // AddMediatR() requires MediatR.Extensions.Microsoft.DependencyInjection
            serviceCollection.AddMediatR(typeof(Startup).Assembly);

            serviceCollection
                .AddDbContext<ExampleContext>(o =>
                {
                    // UseSqlServer() requires Microsoft.EntityFrameworkCore.SqlServer
                    o.UseSqlServer(configuration.GetConnectionString("SetupReference")); //, x => x.UseNetTopologySuite());
                });


            return serviceCollection;
        }

        public static IServiceCollection AddDataProtectionWithAzure(this IServiceCollection serviceCollection, IConfiguration dataProtectionConfiguration)
        {
            var blobStorageConfiguration = dataProtectionConfiguration.GetSection("BlobStorage");
            var keyVaultConfiguration = dataProtectionConfiguration.GetSection("KeyVault");

            serviceCollection
                .AddDataProtection()
                .SetApplicationName("SetupReference");
                //.PersistKeysToAzureBlobStorage(
                //    blobStorageConfiguration["ConnectionString"],
                //    blobStorageConfiguration["Container"],
                //    blobStorageConfiguration["Blob"]
                //)
                //.ProtectKeysWithAzureKeyVault(
                //    new Uri(keyVaultConfiguration["KeyIdentifier"])
                //    // Requires Azure.Identity
                //    //, new ClientSecretCredential(
                //    //    keyVaultConfiguration["TenantId"],
                //    //    keyVaultConfiguration["ClientId"],
                //    //    keyVaultConfiguration["ClientSecret"]
                //    //)
                //);

            return serviceCollection;
        }

        public static IServiceCollection AddModelBindErrorLogging(this IServiceCollection serviceCollection)
        {
            // Log validation errors when model binding fails. See: https://github.com/dotnet/AspNetCore.Docs/issues/12157
            serviceCollection.PostConfigure<ApiBehaviorOptions>(options =>
            {
                var builtInFactory = options.InvalidModelStateResponseFactory;

                options.InvalidModelStateResponseFactory = context =>
                {
                    // Get the factory rather than an instance so that we can...
                    var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();

                    // Use the Action namespace details for the context rather than 'ServiceCollectionExtensions'
                    var logger = loggerFactory.CreateLogger(context.ActionDescriptor.DisplayName);

                    context.ModelState.SelectMany(x => x.Value.Errors)
                                      .ToList()
                                      .ForEach(error => logger.LogWarning(error.ErrorMessage));

                    return builtInFactory(context);
                };
            });

            return serviceCollection;
        }

        public static IServiceCollection AddCurrentProject(this IServiceCollection services)
        {
            //services
            //    .AddSingleton<IValidator<T1>, T2>();

            return services;
        }
    }
}
