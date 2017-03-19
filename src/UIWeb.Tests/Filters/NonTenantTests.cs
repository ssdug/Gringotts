using System.Web.Mvc;
using System.Web.Routing;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Tests.Filters
{
    [TestClass]
    public class NonTenantTests
    {
        private ActionFilterAttribute filter;


        [TestInitialize]
        public void Init()
        {
            filter = new NonTenant
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod]
        public void Should_strip_tenant_key_from_route_data_for_non_tenant_controllers()
        {
            var context = Substitute.For<ActionExecutingContext>();
            context.RouteData = new RouteData { Values = {{ "tenant","foo" }}};
            context.Controller = new NonTenantController();

            filter.OnActionExecuting(context);

            var result = context.Result as RedirectToRouteResult;

            Assert.IsNotNull(result);

            Assert.IsFalse(result.RouteValues.ContainsKey("tenant"));

        }

        [TestMethod]
        public void Should_do_nothing_when_tenant_route_is_not_available_for_non_tenant_controllers()
        {
            var context = Substitute.For<ActionExecutingContext>();
            context.RouteData = new RouteData();
            context.Controller = new NonTenantController();

            filter.OnActionExecuting(context);

            Assert.IsNull(context.Result);
        }

        [TestMethod]
        public void Should_do_nothing_when_tenant_route_is_available_for_tenant_controllers()
        {
            var context = Substitute.For<ActionExecutingContext>();
            context.RouteData = new RouteData { Values = { { "tenant", "foo" } } };
            context.Controller = new TenantController();

            filter.OnActionExecuting(context);

            Assert.IsNull(context.Result);
        }

        [Tenant]
        class TenantController : Controller
        { }

        class NonTenantController : Controller
        { }
    }
}