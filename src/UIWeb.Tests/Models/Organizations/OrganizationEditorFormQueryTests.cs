using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Helpers;
using Wiz.Gringotts.UIWeb.Models;
using Wiz.Gringotts.UIWeb.Models.Features;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Models.Organizations
{
    [TestClass]
    public class OrganizationEditorFormQueryTests
    {
        private ILookup<Feature> lookup;
        private OrganizationEditorFormQueryHandler handler;
        private Feature[] features;
        private ISearch<Organization> organizations;

        [TestInitialize]
        public void Init()
        {
            lookup = Substitute.For<ILookup<Feature>>();
            organizations = Substitute.For<ISearch<Organization>>();
            handler = new OrganizationEditorFormQueryHandler(organizations, lookup)
            {
                Logger = Substitute.For<ILogger>()
            };
            features = new[] {new Feature {Id = 1, Name = Feature.Funds}};
            lookup.All.Returns(features.AsEnumerable());
        }

        [TestMethod]
        public async Task Can_get_form()
        {
            var query = new OrganizationEditorFormQuery();

            var result = await handler.Handle(query);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OrganizationEditorForm));
        }

        [TestMethod]
        public async Task Can_get_form_for_organization()
        {
            var id = new int?(42);
            var query = new OrganizationEditorFormQuery(organizationId: id);

            organizations.GetById(Arg.Any<int>())
                .Returns(new[] {new Organization {Id = id.Value}}.AsAsyncQueryable());

            var result = await handler.Handle(query);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OrganizationEditorForm));
            Assert.AreEqual(result.OrganizationId, id);
        }

        [TestMethod]
        public async Task Can_get_form_for_parent_organization()
        {
            var id = new int?(42);
            var query = new OrganizationEditorFormQuery(parentOrganizationId: id);

            organizations.GetById(Arg.Any<int>())
                .Returns(new[] { new Organization { Id = id.Value } }.AsAsyncQueryable());

            var result = await handler.Handle(query);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OrganizationEditorForm));
            Assert.AreEqual(result.ParentOrganizationId, id);
        }

        [TestMethod]
        public async Task Missing_organization_generates_warning()
        {
            var id = new int?(42);
            var query = new OrganizationEditorFormQuery(organizationId: id);

            organizations.GetById(Arg.Any<int>())
                .Returns(Enumerable.Empty<Organization>().AsAsyncQueryable());

            await handler.Handle(query);

            handler.Logger.Received().Warn(Arg.Any<string>(), id);
        }

        [TestMethod]
        public async Task Missing_parent_organization_generates_warning()
        {
            var id = new int?(42);
            var query = new OrganizationEditorFormQuery(parentOrganizationId: id);

            organizations.GetById(Arg.Any<int>())
                .Returns(Enumerable.Empty<Organization>().AsAsyncQueryable());

            await handler.Handle(query);

            handler.Logger.Received().Warn(Arg.Any<string>(), id);
        }
    }
}