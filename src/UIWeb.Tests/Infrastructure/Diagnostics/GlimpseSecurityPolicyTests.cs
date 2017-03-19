using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Wiz.Gringotts.UIWeb.Infrastructure;
using Wiz.Gringotts.UIWeb.Infrastructure.Diagnostics;
using Glimpse.Core.Extensibility;
using Http.TestLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Tests.Infrastructure.Diagnostics
{
    [TestClass]
    public class GlimpseSecurityPolicyTests
    {
        private IApplicationConfiguration config;
        private HttpSimulator httpSimulator;

        [TestInitialize]
        public void Init()
        {
            httpSimulator = new HttpSimulator().SimulateRequest();
            config = Substitute.For<IApplicationConfiguration>();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(config);

            DependencyResolver.SetResolver(new AutofacDependencyResolver(builder.Build()));
        }

        [TestCleanup]
        public void Cleanup()
        {
            httpSimulator.Dispose();
        }

        [TestMethod]
        public void Enabled()
        {
            config.EnableGlimpse.Returns(true);

            var result = new GlimpseSecurityPolicy().Execute(null);

            Assert.AreEqual(result, RuntimePolicy.On);
        }

        [TestMethod]
        public void Disabled()
        {
            config.EnableGlimpse.Returns(false);

            var result = new GlimpseSecurityPolicy().Execute(null);

            Assert.AreEqual(result, RuntimePolicy.Off);
        }

        [TestMethod]
        public void Should_execute_on_end_request()
        {
            var result = new GlimpseSecurityPolicy().ExecuteOn;

            Assert.IsTrue(result.HasFlag(RuntimeEvent.EndRequest));
        }

        [TestMethod]
        public void Should_execute_on_execute_resource()
        {
            var result = new GlimpseSecurityPolicy().ExecuteOn;

            Assert.IsTrue(result.HasFlag(RuntimeEvent.ExecuteResource));
        }
    }
}