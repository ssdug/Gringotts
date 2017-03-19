using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.ActiveDirectory;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Wiz.Gringotts.UIWeb.Models.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Tests.Models.Organizations
{
    [TestClass]
    public class OrganizationDetailsViewModelPostProcessorTests
    {
        private IUserRepository userRepository;
        private OrganizationDetailsViewModelPostProccesor handler;

        [TestInitialize]
        public void Init()
        {
            userRepository = Substitute.For<IUserRepository>();
            handler = new OrganizationDetailsViewModelPostProccesor(userRepository)
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod]
        public async Task Null_resposes_do_nothing()
        {
            var request = new OrganizationDetailsQuery(42);
            var response = null as OrganizationDetails;

            await handler.Handle(request, response);

            Assert.IsNull(response);
        }

        [TestMethod]
        public async Task Can_populate_fiscal_contact()
        {
            var user = new User();
            var request = new OrganizationDetailsQuery(42);
            var response = new OrganizationDetails
            {
                Organization = new Organization { FiscalContactSamAccountName = "foo" }
            };

            userRepository.FindByUser(Arg.Any<string>())
                .Returns(user);

            await handler.Handle(request, response);

            Assert.AreEqual(response.FiscalContact, user);
        }

        [TestMethod]
        public async Task Can_populate_it_contact()
        {
            var user = new User();
            var request = new OrganizationDetailsQuery(42);
            var response = new OrganizationDetails
            {
                Organization = new Organization { ITConactSamAccountName = "foo" }
            };

            userRepository.FindByUser(Arg.Any<string>())
                .Returns(user);

            await handler.Handle(request, response);

            Assert.AreEqual(response.ITContact, user);
        }

        [TestMethod]
        public async Task Can_populate_users()
        {
            var user = new User();
            var request = new OrganizationDetailsQuery(42);
            var response = new OrganizationDetails
            {
                Organization = new Organization { GroupName = "foo" }
            };

            userRepository.FindByOrganization(Arg.Any<string>())
                .Returns(new [] {user});

            await handler.Handle(request, response);

            Assert.IsTrue(response.Users.Contains(user));
        }
    }
}