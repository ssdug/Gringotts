using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Data.Interceptors;
using Wiz.Gringotts.UIWeb.Data.Migrations;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Tests.Helpers
{
    public class TestConfiguration : Configuration
    {
        public TestConfiguration()
        {
            MigrationsAssembly = typeof (ApplicationDbContext).Assembly;
            MigrationsNamespace = typeof (InitialState).Namespace;
        }
    }

    public static class TestDbContextFactory
    {
        public static class Providers
        {
            public const string SqlServerCe = "System.Data.SqlServerCe.4.0";
            public const string SqlServer = "System.Data.SqlClient";
        }

        private static readonly IDictionary<string, string> ProviderConnections = new Dictionary<string, string>
        {
            {Providers.SqlServerCe, "Data Source={0}.sdf"},
            {Providers.SqlServer, "data source=.;initial catalog={0};integrated security=True;MultipleActiveResultSets=True;"}

        };

        public static InterceptingApplicationDbContext Build(string provider = Providers.SqlServer, IEnumerable<IInterceptor> interceptors = null)
        {
            var databaseName = DateTime.Now.Ticks.ToString();
            var connectionString = ProviderConnections[provider]
                .FormatWith(databaseName);

            StandupDatabase(provider, connectionString);

            return CreateDbContext(provider, connectionString, interceptors);
        }

        private static void StandupDatabase(string provider, string connectionString)
        {
            var config = new TestConfiguration
            {
                TargetDatabase = new DbConnectionInfo(connectionString, provider),
                AutomaticMigrationsEnabled = true
            };

            var migrator = new DbMigrator(config);

            migrator.Update();
        }

        private static InterceptingApplicationDbContext CreateDbContext(string provider, string connectionString, IEnumerable<IInterceptor> interceptors)
        {
            var connection = DbProviderFactories.GetFactory(provider)
                                .CreateConnection();

            connection.ConnectionString = connectionString;

            return new InterceptingApplicationDbContext(new Lazy<IEnumerable<IInterceptor>>(() => interceptors ?? Enumerable.Empty<IInterceptor>()), connection);
        }
    }
}