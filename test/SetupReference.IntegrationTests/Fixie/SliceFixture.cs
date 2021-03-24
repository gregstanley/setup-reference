using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FakeItEasy;
using MediatR;
using Respawn;
using SetupReference.Database;
using SetupReference.Database.Example;

using static SetupReference.IntegrationTests.Fixie.IntegrationTestsConfigurationProvider;

namespace SetupReference.IntegrationTests.Fixie
{
    // Built referencing historical Fixie example here:
    // https://github.com/jbogard/ContosoUniversityCore/tree/f3a0b664fb71da89276db87b1a7e8d2bd24cd96a/test/ContosoUniversityCore.IntegrationTests
    internal class SliceFixture
    {
        public static readonly int TenantId = 1;

        private static readonly string BaseConnectionString = "Server=(localdb)\\mssqllocaldb;Integrated Security=True";
        private static readonly string DatabaseNamePrefix = "ExampleIntegration";

        private static readonly IServiceScopeFactory scopeFactory;
        private static readonly string connectionString;

        private static readonly Checkpoint checkpoint = new Checkpoint
        {
            TablesToIgnore = new[]
            {
                "sysdiagrams",
                "tblUser",
                "tblObjectType",
            }
        };

        static SliceFixture()
        {
            connectionString = DatabaseConnectionHelpers.GenerateConnectionString(BaseConnectionString, DatabaseNamePrefix);

            var integrationConfiguration = new ConfigurationBuilder()
                .Add(new IntegrationTestsConfiguration(connectionString))
                .Build();

            var services = new ServiceCollection();

            // We're not dealing with the MVC level so only setup the service layer dependecies
            services.AddServiceLayerServices(integrationConfiguration);

            AddFakeTenantProvider(services);

            scopeFactory = services.BuildServiceProvider().GetService<IServiceScopeFactory>();
        }

        public static async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            using var scope = scopeFactory.CreateScope();
            await action(scope.ServiceProvider);
        }

        public static async Task<TResponse> ExecuteScopeAsync<TResponse>(Func<IServiceProvider, Task<TResponse>> action)
        {
            using var scope = scopeFactory.CreateScope();
            var result = await action(scope.ServiceProvider);
            return result;
        }

        public static Task ExecuteContextAsync<TContext>(Func<TContext, Task> action) where TContext : DbContext =>
            ExecuteScopeAsync(sp => action(sp.GetService<TContext>()));

        public static Task<TResponse> ExecuteContextAsync<TContext, TResponse>(Func<TContext, Task<TResponse>> action) where TContext : DbContext =>
            ExecuteScopeAsync(sp => action(sp.GetService<TContext>()));

        public static Task RecreateAndMigrateDatabaseAsync() =>
            ExecuteScopeAsync(async sp =>
            {
                var exampleContext = sp.GetService<ExampleContext>();

                await exampleContext.Database.EnsureDeletedAsync();

                // Contexts share database meaning EnsureCreated can only build one of them (a second call is ignored)
                // await exampleContext.Database.MigrateAsync();
                await exampleContext.Database.EnsureCreatedAsync();
            });

        public static async Task ResetCheckpoint() => await checkpoint.Reset(connectionString);

        public static Task DeleteDatabaseAsync() =>
            ExecuteScopeAsync(async sp =>
            {
                var exampleContext = sp.GetService<ExampleContext>();
                await exampleContext.Database.EnsureDeletedAsync();
            });

        public static Task InsertAsync<TContext>(params object[] entities) where TContext : DbContext =>
            ExecuteContextAsync<TContext>(context => InsertAsync(context, entities));

        public static Task SendAsync(IRequest request) =>
            ExecuteScopeAsync(sp =>
            {
                var mediator = sp.GetService<IMediator>();
                return mediator.Send(request);
            });

        public static Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request) =>
            ExecuteScopeAsync(sp =>
            {
                var mediator = sp.GetService<IMediator>();
                return mediator.Send(request);
            });

        private static void AddFakeTenantProvider(IServiceCollection services)
        {
            var tenantProvider = A.Fake<ITenantProvider<int>>();

            A.CallTo(() => tenantProvider.TenantId).Returns(TenantId);

            services.AddScoped(x => tenantProvider);
        }

        private static Task InsertAsync(DbContext context, params object[] entities)
        {
            foreach (var entity in entities)
                context.Add(entity);

            return context.SaveChangesAsync();
        }
    }
}
