using System.Data.Entity;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using MediatR;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Models.Payees
{
    public class TogglePayeeIsActiveCommand : IAsyncRequest<ICommandResult>
    {
        public int PayeeId { get; private set; }

        public TogglePayeeIsActiveCommand(int payeeId)
        {
            this.PayeeId = payeeId;
        }
    }

    public class TogglePayeeIsActiveCommandHandler : IAsyncRequestHandler<TogglePayeeIsActiveCommand, ICommandResult>
    {
        public ILogger Logger { get; set; }

        private readonly ISearch<Payee> payees;
        private readonly ApplicationDbContext context;

        public TogglePayeeIsActiveCommandHandler(ISearch<Payee> payees, ApplicationDbContext context)
        {
            this.payees = payees;
            this.context = context;
        }

        public async Task<ICommandResult> Handle(TogglePayeeIsActiveCommand command)
        {
            Logger.Trace("Handle::{0}", command.PayeeId);

            var payee = await payees.GetById(command.PayeeId)
                .FirstOrDefaultAsync();

            if (payee != null)
            {
                payee.IsActive = !payee.IsActive;
                await context.SaveChangesAsync();
                Logger.Info("Handle::Success Id:{0} IsActive:{1}", payee.Id, payee.IsActive);
                return new SuccessResult(payee.Id);
            }

            return new FailureResult("Payee {0} not found.".FormatWith(command.PayeeId));
        }
    }
}