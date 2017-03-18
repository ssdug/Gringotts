using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.Routing;
using Wiz.Gringotts.UIWeb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb
{
    [TestClass]
    public class RouteConfigTests
    {
        private RouteCollection routes;


        [TestInitialize]
        public void Init()
        {
            routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);
        }

        [TestMethod]
        public void Can_map_default_route()
        {
            AssertRoute(routes, "~/", new { Controller = "Root", Action = "Index"});
            AssertRoute(routes, "~/Root/WhoAmI", new { Controller = "Root", Action = "WhoAmI"});
            AssertRoute(routes, "~/Organizations/Index/2", new { Controller = "Organizations", Action = "Index", Id = 2 });
        }

        [TestMethod]
        public void Can_map_tenant_route()
        {
            AssertRoute(routes, "~/Dev/Root/Index", new { Controller = "Root", Action = "Index", Tenant = "Dev" });
            AssertRoute(routes, "~/Dev/Root/Index/2", new { Controller = "Root", Action = "Index", Tenant = "Dev", Id = 2 });
        }

        [TestMethod]
        public void Can_map_select_tenant_route()
        {
            AssertRoute(routes, "~/Organizations/Select", new { Controller = "Organizations", Action = "Select" });
            AssertRoute(routes, "~/Organizations/Select/foo", new { Controller = "Organizations", Action = "Select", tenantKey = "foo" });
        }

        private static void AssertRoute(RouteCollection routes, string url, object expectations)
        {
            var httpContext = Substitute.For<HttpContextBase>();

            httpContext.Request.AppRelativeCurrentExecutionFilePath.Returns(url);

            var routeData = routes.GetRouteData(httpContext);

            Assert.IsNotNull(routeData, "should have found the route");
            
            foreach (PropertyValue property in GetProperties(expectations))
            {
                Assert.IsTrue(string.Equals(property.Value.ToString(),
                    routeData.Values[property.Name].ToString(),
                    StringComparison.OrdinalIgnoreCase)
                    , string.Format("Expected '{0}', not '{1}' for '{2}'.",
                        property.Value, routeData.Values[property.Name], property.Name));
            }
        }

        private static IEnumerable<PropertyValue> GetProperties(object o)
        {
            if (o != null)
            {
                var props = TypeDescriptor.GetProperties(o);
                foreach (PropertyDescriptor prop in props)
                {
                    var val = prop.GetValue(o);
                    if (val != null)
                    {
                        yield return new PropertyValue { Name = prop.Name, Value = val };
                    }
                }
            }
        }

        private sealed class PropertyValue
        {
            public string Name { get; set; }
            public object Value { get; set; }
        }

    }
}