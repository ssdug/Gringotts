using System.Data.Entity;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using MediatR;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Models.ReceiptSources
{
    public class ToggleReceiptSourceIsActiveCommand : IAsyncRequest<ICommandResult>
    {
        public int ReceiptSourceId { get; private set; }

        public ToggleReceiptSourceIsActiveCommand(int receiptSourceId)
        {
            this.ReceiptSourceId = receiptSourceId;
        }
    }

    public class ToggleReceiptSourceIsActiveCommandHandler : IAsyncRequestHandler<ToggleReceiptSourceIsActiveCommand, ICommandResult>
    {
        private readonly ISearch<ReceiptSource> sources;
        private readonly IUnitOfWork unitOfWork;
        public ILogger Logger { get; set; }

        public ToggleReceiptSourceIsActiveCommandHandler(ISearch<ReceiptSource> sources, IUnitOfWork unitOfWork)
        {
            this.sources = sources;
            this.unitOfWork = unitOfWork;
        }

        public async Task<ICommandResult> Handle(ToggleReceiptSourceIsActiveCommand command)
        {
            Logger.Trace("Handle::{0}", command.ReceiptSourceId);

            var source = await sources.GetById(command.ReceiptSourceId).FirstOrDefaultAsync();

            if (source == null)
                return new FailureResult("Receipt Source {0} not found.".FormatWith(command.ReceiptSourceId));

            source.IsActive = !source.IsActive;
            await unitOfWork.SaveChangesAsync();
            Logger.Info("Handle::Success Id:{0} IsActive:{1}", source.Id, source.IsActive);

            return new SuccessResult(source.Id);
        }
    }
}