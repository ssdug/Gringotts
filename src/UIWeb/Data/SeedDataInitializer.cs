using System.Data.Entity;
using Wiz.Gringotts.UIWeb.Data.Interceptors;
using Wiz.Gringotts.UIWeb.Data.Migrations;

namespace Wiz.Gringotts.UIWeb.Data
{
    public class SeedDataInitializer : IDatabaseInitializer<ApplicationDbContext>, IDatabaseInitializer<InterceptingApplicationDbContext>
    {
        public void InitializeDatabase(ApplicationDbContext context)
        {
            SeedData.Execute(context);
        }

        public void InitializeDatabase(InterceptingApplicationDbContext context)
        {
            SeedData.Execute(context);
        }
    }
}