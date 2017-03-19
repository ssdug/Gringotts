using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Tests.Helpers;
using Wiz.Gringotts.UIWeb.Data.Interceptors;
using Wiz.Gringotts.UIWeb.Infrastructure.Security;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Tests.Data.Interceptors
{
    [TestClass]
    public class AuditChangeInterceptorTests
    {
        private InterceptingApplicationDbContext context;
        private IPrincipal principal;
        private IPrincipalProvider principalProvider;
        private AuditChangeInterceptor interceptor;

        [TestInitialize]
        public void Init()
        {
            
            principalProvider = Substitute.For<IPrincipalProvider>();
            principal = Substitute.For<IPrincipal>();


            principalProvider.GetCurrent().Returns(principal);
            principal.Identity.Name.Returns("user");

            interceptor = new AuditChangeInterceptor(principalProvider)
            {
                Logger = Substitute.For<ILogger>()
            };

            context = TestDbContextFactory.Build(interceptors: new[] {interceptor});
        }

        [TestCleanup]
        public void Cleanup()
        {
            context.Database.Delete();
        }

        [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.Database)]
        public void Insert_Organization_populates_all_audit_fields()
        {
            var org = new Organization
            {
                Id = -1,
                GroupName = "foo",
                Name = "foo",
                Abbreviation = "foo",
                AddressLine1 = "foo",
                City = "foo",
                State = "foo",
                PostalCode = "foo",
                Phone = "260-555-1212",
                FiscalContactSamAccountName = "sallyj",
                ITConactSamAccountName = "johnsb"
            };

            context.Organizations.Add(org);

            context.SaveChanges();

            Assert.AreEqual(org.CreatedBy, "user");
            Assert.AreEqual(org.UpdatedBy, "user");

            Assert.IsTrue(org.Created > DateTime.UtcNow.AddMinutes(-1));
            Assert.IsTrue(org.Updated > DateTime.UtcNow.AddMinutes(-1));

        }

        [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.Database)]
        public async Task Update_Organization_populates_update_audit_fields()
        {
            var org = new Organization
            {
                Id = -1,
                GroupName = "foo",
                Name = "foo",
                Abbreviation = "foo",
                AddressLine1 = "foo",
                City = "foo",
                State = "foo",
                PostalCode = "foo",
                Phone = "260-555-1212",
                FiscalContactSamAccountName = "sallyj",
                ITConactSamAccountName = "johnsb"
            };

            context.Organizations.Add(org);

            await context.SaveChangesAsync();

            var insertUpdatedDate = org.Updated;

            org.FiscalContactSamAccountName = "test";
            org.UpdatedBy = null;
            org.Updated = DateTime.MinValue;

            await context.SaveChangesAsync();

            Assert.AreEqual(org.UpdatedBy, "user");

            Assert.IsTrue(org.Updated > insertUpdatedDate);

        }
         
    }
}