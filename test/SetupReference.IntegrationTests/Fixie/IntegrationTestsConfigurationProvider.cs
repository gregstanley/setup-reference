using Microsoft.Extensions.Configuration;

namespace SetupReference.IntegrationTests.Fixie
{
    public class IntegrationTestsConfigurationProvider : ConfigurationProvider
    {
        private IntegrationTestsConfigurationProvider(string connectionString)
        {
            Set("ConnectionStrings:SetupReference", connectionString);
        }

        public class IntegrationTestsConfiguration : IConfigurationSource
        {
            private readonly string connectionString;

            public IntegrationTestsConfiguration(string connectionString)
            {
                this.connectionString = connectionString;
            }

            public IConfigurationProvider Build(IConfigurationBuilder builder) =>
                new IntegrationTestsConfigurationProvider(connectionString);
        }
    }
}
