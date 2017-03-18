using System.Threading.Tasks;
using Autofac.Extras.NLog;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Infrastructure.Mediation
{
    public class AsyncMediatorPipeline<TRequest, TResponse> : IAsyncRequestHandler<TRequest, TResponse> where TRequest : IAsyncRequest<TResponse>
    {
        public ILogger Logger { get; set; }

        private readonly IAsyncRequestHandler<TRequest, TResponse> inner;
        private readonly IAsyncPreRequestHandler<TRequest>[] preRequestHandlers;
        private readonly IAsyncPostRequestHandler<TRequest, TResponse>[] postRequestHandlers;

        public AsyncMediatorPipeline(IAsyncRequestHandler<TRequest, TResponse> inner, 
            IAsyncPreRequestHandler<TRequest>[] preRequestHandlers, 
            IAsyncPostRequestHandler<TRequest, TResponse>[] postRequestHandlers)
        {
            this.inner = inner;
            this.preRequestHandlers = preRequestHandlers;
            this.postRequestHandlers = postRequestHandlers;
        }

        public async Task<TResponse> Handle(TRequest message)
        {
            Logger.Trace("Handle");

            foreach (var preRequestHandler in preRequestHandlers)
            {
                 await preRequestHandler.Handle(message);
            }

            var result = await inner.Handle(message);


            if (result != null)
            {
                foreach (var postRequestHandler in postRequestHandlers)
                {
                    await postRequestHandler.Handle(message, result);
                }
            }

            return result;
        }
    }
}