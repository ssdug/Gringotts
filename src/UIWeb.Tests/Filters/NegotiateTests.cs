using System.Collections.Specialized;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Tests.Filters
{
    [TestClass]
    public class NegotiateTests
    {
        private Negotiate filter;
        private ActionExecutedContext context;

        [TestInitialize]
        public void Init()
        {
            context = Substitute.For<ActionExecutedContext>();
            context.HttpContext = Substitute.For<HttpContextBase>();
            context.HttpContext.Request.Returns(Substitute.For<HttpRequestBase>());
            context.HttpContext.Request.QueryString.Returns(new NameValueCollection());
            context.Controller = new TestController
            {
                ViewData = new ViewDataDictionary()
            };
            filter = new Negotiate
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod]
        public void Should_not_serialize_html_requests()
        {
            var actionResult = new ViewResult();
            context.Result = actionResult;
            context.HttpContext.Request
                .AcceptTypes.Returns(new[] { "text/html" });

            filter.OnActionExecuted(context);

            Assert.AreSame(context.Result, actionResult);
        }

        [TestMethod]
        public void Should_serialize_json_requests()
        {
            var model = new {Id = 42, Name = "Foo"};
            context.Controller.ViewData.Model = model;
            context.HttpContext.Request
                .AcceptTypes.Returns(new[] { "application/json" });

            filter.OnActionExecuted(context);

            Assert.IsNotNull(context.Result);
            Assert.IsInstanceOfType(context.Result, typeof(ContentResult));
            
            var result = Json.Decode(((ContentResult) context.Result).Content);

            Assert.AreEqual(result.Id, model.Id);
            Assert.AreEqual(result.Name, model.Name);
        }

        [TestMethod]
        public void Should_respect_fields_query_string_values()
        {
            var model = new { Id = 42, Name = "Foo", GiantObject = "some giant object we dont want" };
            context.Controller.ViewData.Model = model;
            
            context.HttpContext.Request
                .AcceptTypes.Returns(new[] { "application/json" });

            context.HttpContext.Request.QueryString.Add("fields","Id,Name");

            filter.OnActionExecuted(context);

            Assert.IsNotNull(context.Result);
            Assert.IsInstanceOfType(context.Result, typeof(ContentResult));

            var result = Json.Decode(((ContentResult)context.Result).Content);

            Assert.AreEqual(result.Id, model.Id);
            Assert.AreEqual(result.Name, model.Name);
            Assert.IsNull(result.GiantObject);
        }

        public class TestController : Controller
        { }
    }
}
