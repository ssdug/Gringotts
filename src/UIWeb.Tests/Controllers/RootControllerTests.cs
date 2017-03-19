using System.Security.Principal;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Controllers;
using Wiz.Gringotts.UIWeb.Infrastructure.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Tests.Controllers
{
    [TestClass]
    public class RootControllerTests
    {
        private RootController controller;
        private IPrincipalProvider principalProvider;
        private IPrincipal principal;
        private IIdentity identity;

        [TestInitialize]
        public void Init()
        {
            identity = Substitute.For<IIdentity>();
            principal = Substitute.For<IPrincipal>();
            principalProvider = Substitute.For<IPrincipalProvider>();

            principal.Identity.Returns(identity);
            principalProvider.GetCurrent().Returns(principal);

            controller = new RootController(principalProvider)
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestCleanup]
        public void Cleanup()
        {
            controller.Dispose();
            controller = null;
        }

        [TestMethod]
        public void Index_Displays_Index()
        {
            var result = controller.Index();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhoAmI_defaults_to_current_user()
        {
            const string name = "name";
            identity.Name.Returns(name);

            var result = controller.WhoAmI(string.Empty) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ViewBag.Title, "Who Am I?");
            Assert.AreEqual(result.Model, name);

            principalProvider.Received().GetCurrent();
        }

        [TestMethod]
        public void WhoAmI_shows_any_user_name()
        {
            const string name = "name";

            var result = controller.WhoAmI(name) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.ViewBag.Title, "Who Are They?");
            Assert.AreEqual(result.Model, name);

            principalProvider.DidNotReceive().GetCurrent();
        }
         
    }
}