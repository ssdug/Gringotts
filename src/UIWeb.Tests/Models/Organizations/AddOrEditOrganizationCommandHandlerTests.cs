using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Helpers;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Models;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Models.Organizations
{
    [TestClass]
    public class AddOrEditOrganizationCommandHandlerTests
    {
        private AddOrEditOrganizationCommandHandler handler;
        private ApplicationDbContext context;
        private ISearch<Organization> organizations;

        [TestInitialize]
        public void Init()
        {
            context = Substitute.For<ApplicationDbContext>();
            context.Organizations.Initialize(Enumerable.Empty<Organization>().AsQueryable());
            organizations = Substitute.For<ISearch<Organization>>();
            handler = new AddOrEditOrganizationCommandHandler(organizations, context)
            {
                Logger = Substitute.For<ILogger>()
            };

        }

        [TestMethod]
        public async Task Can_add_an_organization()
        {
            var command = GetCommand(add: true);
            
            var result = await handler.Handle(command);

            Assert.IsInstanceOfType(result, typeof (SuccessResult));
            
            context.Organizations.Received()
                .Add(Arg.Any<Organization>());

            context.Received().SaveChangesAsync()
                .IgnoreAwaitForNSubstituteAssertion();
        }

        [TestMethod]
        public async Task Can_edit_an_organization()
        {
            var command = GetCommand(add: false);
            
            organizations.GetById(Arg.Any<int>())
                .Returns(new[] {new Organization {Id = 42}}.AsAsyncQueryable());

            var result = await handler.Handle(command);

            Assert.IsInstanceOfType(result, typeof(SuccessResult));

            
            context.Received().SaveChangesAsync()
                .IgnoreAwaitForNSubstituteAssertion();
        }

        [TestMethod]
        public async Task ModelState_errors_cause_failure()
        {
            var command = GetCommand(add: false);

            command.ModelState.AddModelError("foo","bar");

            var result = await handler.Handle(command);

            Assert.IsInstanceOfType(result, typeof(FailureResult));
        }

        private AddOrEditOrganizationCommand GetCommand(bool add = false)
        {
            var form = GetForm(add: add);

            return new AddOrEditOrganizationCommand(form, new ModelStateDictionary());
        }

        private static OrganizationEditorForm GetForm(bool add = false)
        {
            return new OrganizationEditorForm
            {
                OrganizationId = add ? new int?() : 42,
            };
        }
    }
}