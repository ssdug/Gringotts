using System.Data.Entity;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using MediatR;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Models.Accounts
{
    public class ToggleAccountIsActiveCommand : IAsyncRequest<ICommandResult>
    {
        public int? AccountId { get; private set; }

        public ToggleAccountIsActiveCommand(int? accountId)
        {
            this.AccountId = accountId;
        }
    }

    public class ToggleAccountIsActiveCommandHandler : IAsyncRequestHandler<ToggleAccountIsActiveCommand, ICommandResult>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ISearch<Account> accounts;
        public ILogger Logger { get; set; }

        public ToggleAccountIsActiveCommandHandler(IUnitOfWork unitOfWork,
            ISearch<Account> accounts)
        {
            this.unitOfWork = unitOfWork;
            this.accounts = accounts;
        }

        public async Task<ICommandResult> Handle(ToggleAccountIsActiveCommand message)
        {
            Logger.Trace("Handle");

            var account = await accounts.GetById(message.AccountId ?? -1)
                .FirstOrDefaultAsync();

            if (account is ClientAccount)
                return new FailureResult("Client Accounts may not be toggled directly.");

            if (account != null)
            {
                account.IsActive = !account.IsActive;
                await unitOfWork.SaveChangesAsync();
                Logger.Info("Handle::Success Id:{0} IsActive:{1}", account.Id, account.IsActive);
                return new SuccessResult(account.Id);
            }

            return new FailureResult("Account {0} not found.".FormatWith(message.AccountId));
        }
    }
}