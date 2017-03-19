using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Filters;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Tests.Filters
{
    [TestClass]
    public class TenantTests
    {
        private ITenantOrganizationProvider provider;
        private Tenant filter;

        [TestInitialize]
        public void Init()
        {
            provider = Substitute.For<ITenantOrganizationProvider>();
            filter = new Tenant
            {
                Logger = Substitute.For<ILogger>(),
                TenantOrganizationProvider = provider,
                TenantUrlHelper = Substitute.For<ITenantUrlHelper>()
            };
        }

        [TestMethod]
        public void Should_add_tenant_to_viewbag_if_tenant_route_exists()
        {
            var organization = new Organization{Id = 42};
            var context = Substitute.For<ActionExecutingContext>();
            var controller = Substitute.For<Controller>();
            context.Controller.Returns(controller);
            provider.GetTenantOrganization().Returns(organization);
            context.RouteData = new RouteData { Values = { { "tenant", "foo" } } };

            filter.OnActionExecuting(context);
            
            Assert.IsNull(context.Result);
            Assert.AreEqual(context.Controller.ViewBag.TenantBreadcrumbs.CurrentTenantOrganization, organization);
        }

        [TestMethod]
        public void Should_redirect_if_tenant_route_does_not_exist_and_user_has_a_tenant_organization()
        {
            var context = Substitute.For<ActionExecutingContext>();
            var organization = new Organization { Name = "foo" };

            context.RouteData = new RouteData();

            filter.TenantOrganizationProvider.GetTenantOrganization()
                .Returns(organization);

            filter.TenantUrlHelper.GetRedirectUrl(Arg.Any<Organization>())
                .Returns("someurl");

            filter.OnActionExecuting(context);

            var result = context.Result as RedirectResult;

            Assert.IsNotNull(result);

            filter.TenantUrlHelper.Received()
                .GetRedirectUrl(organization);
        }

        [TestMethod]
        public void Should_redirect_to_select_tenant_if_tenant_route_does_not_exist_and_user_has_no_tenant_organization()
        {
            var context = Substitute.For<ActionExecutingContext>();
            
            context.RouteData = new RouteData();

            filter.OnActionExecuting(context);

            var result = context.Result as RedirectToRouteResult;

            Assert.IsNotNull(result);

            Assert.IsTrue(result.RouteValues.ContainsKey("ReturnUrl"));

            filter.TenantUrlHelper.Received()
                .GetReturnUrl();
        }


        [TestMethod]
        public void Should_redirect_if_tenantkey_is_no_longer_available_to_user()
        {
            var context = Substitute.For<ActionExecutingContext>();

            context.RouteData = new RouteData { Values = { { "tenant", "foo" } } };
            context.Controller = new TestController();

            filter.TenantOrganizationProvider.GetTenantOrganization()
                .Returns(null as Organization);

            filter.TenantOrganizationProvider.GetAllTenantOrganizationsForUser()
                .Returns(Enumerable.Empty<Organization>());

            filter.TenantUrlHelper.GetReturnUrl()
                .Returns("someurl");

            filter.OnActionExecuting(context);

            var result = context.Result as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.IsNull(context.Controller.ViewBag.TenantBreadcrumbs);

            filter.TenantUrlHelper.Received()
                .GetReturnUrl();
        }

        private class TestController : Controller
        { }
    }
}