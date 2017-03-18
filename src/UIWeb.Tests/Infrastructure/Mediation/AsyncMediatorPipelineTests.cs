using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Infrastructure.Mediation
{
    [TestClass]
    public class AsyncMediatorPipelineTests
    {
        private class TestRequest : IAsyncRequest<TestResponse>
        {
            public int Value { get; set; }
        }
        private class TestResponse
        {
            public int Value { get; set; }
        }
        private class TestPreRequestHandler : IAsyncPreRequestHandler<TestRequest>
        {
            public Task Handle(TestRequest request)
            {
                return Task.Run(() =>
                {
                    request.Value += 1;
                });
            }
        }
        private class TestPostRequestHandler : IAsyncPostRequestHandler<TestRequest, TestResponse>
        {
            public Task Handle(TestRequest command, TestResponse response)
            {
                return Task.Run(() =>
                {
                    response.Value += 1;
                });
            }
        }
        private class TestRequestHandler : IAsyncRequestHandler<TestRequest, TestResponse>
        {
            public Task<TestResponse> Handle(TestRequest message)
            {
                return Task.Run(() => new TestResponse {Value = 1 + message.Value});
            }
        }

        [TestMethod]
        public async Task Should_process_all_handlers()
        {
            var mediator = new AsyncMediatorPipeline<TestRequest, TestResponse>(new TestRequestHandler(),
                new IAsyncPreRequestHandler<TestRequest>[] { new TestPreRequestHandler() },
                new IAsyncPostRequestHandler<TestRequest, TestResponse>[] { new TestPostRequestHandler() })
            {
                Logger = Substitute.For<ILogger>()
            };

            var response = await mediator.Handle(new TestRequest { Value = 0 });

            Assert.AreEqual(response.Value, 3);
        }

    }
}