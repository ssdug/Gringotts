using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;
using MediatR;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Infrastructure.Commands
{
    public abstract class CommandValidator<T> : IAsyncPreRequestHandler<T> where T : IAsyncRequest<ICommandResult>
    {
        public ILogger Logger { get; set; }
        public IEnumerable<Action<T>> Validators { get; set; }

        public Task Handle(T request)
        {
            Logger.Trace("Handle");

            Validators.ForEach(v => v(request));

            return Task.FromResult(request);  
        }
    }
}