using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;
using System.Data.Entity.SqlServer;

namespace Wiz.Gringotts.UIWeb.Data.Migrations
{
    public class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = "Data\\Migrations";
            SetSqlGenerator("System.Data.SqlClient", new SqlMigrator());
        }

        /// <summary>
        /// This migrator ensures that snapshot isolation is enabled on sql server
        /// </summary>
        private class SqlMigrator : SqlServerMigrationSqlGenerator
        {
            public override IEnumerable<MigrationStatement> Generate(
                IEnumerable<MigrationOperation> migrationOperations, string providerManifestToken)
            {
                var statements = new List<MigrationStatement>
                {
                    new MigrationStatement
                    {
                        Sql = "ALTER DATABASE CURRENT SET READ_COMMITTED_SNAPSHOT ON",
                        SuppressTransaction = true
                    },
                    new MigrationStatement
                    {
                        Sql = "ALTER DATABASE CURRENT SET ALLOW_SNAPSHOT_ISOLATION ON",
                        SuppressTransaction = true
                    }
                };
                statements.AddRange(base.Generate(migrationOperations, providerManifestToken));
                return statements;
            }
        }
    }
}
