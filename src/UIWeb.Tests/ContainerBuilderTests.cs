using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wiz.Gringotts.UIWeb
{
    [TestClass]
    public class ContainerBuilderTests
    {
        private class TestAsyncRequest : IAsyncRequest<TestResult>
        { }

        private class TestResult
        {  }


        private class TestAsyncRequestHandler : IAsyncRequestHandler<TestAsyncRequest, TestResult>
        {
            public Task<TestResult> Handle(TestAsyncRequest message)
            {
                return Task.FromResult(new TestResult());
            }

        }

        [TestMethod]
        public void AsyncMediatorPipeline_Decorator()
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(typeof(TestAsyncRequestHandler).Assembly)
                .Where(type => type.Namespace == "Wiz.Gringotts.UIWeb.Tests")
                .As(type => type.GetInterfaces()
                    .Where(interfaceType => interfaceType.IsClosedTypeOf(typeof (IAsyncRequestHandler<,>)))
                    .Select(interfaceType => new KeyedService("asyncRequestHandler", interfaceType)));

            builder.RegisterGenericDecorator(typeof(AsyncMediatorPipeline<,>), typeof(IAsyncRequestHandler<,>), "asyncRequestHandler");

            var container = builder.Build();

            var handlerType = typeof(IAsyncRequestHandler<,>).MakeGenericType(typeof(TestAsyncRequest), typeof(TestResult));

            var handler = container.Resolve(handlerType);

            Assert.IsNotNull(handler);
            Assert.IsInstanceOfType(handler, typeof(AsyncMediatorPipeline<TestAsyncRequest, TestResult>));
        }

        [TestMethod]
        public void Can_build_container()
        {
            ContainerConfig.RegisterContainer(container =>
            {
                Assert.IsNotNull(container);
                Assert.IsTrue(container.IsRegistered<ILogger>(), "can get a logger");
                Assert.IsTrue(container.IsRegistered<IMediator>(), "can get a mediator");
            });
        }
    }
}