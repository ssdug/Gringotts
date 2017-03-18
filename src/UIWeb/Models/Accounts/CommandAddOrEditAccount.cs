using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;
using Wiz.Gringotts.UIWeb.Models.Funds;
using MediatR;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Models.Accounts
{
    public class AddOrEditAccountCommand : IAsyncRequest<ICommandResult>
    {
        public AccountEditorForm Editor { get; private set; }
        public ModelStateDictionary ModelState { get; private set; }

        public AddOrEditAccountCommand(AccountEditorForm editor, ModelStateDictionary modelState)
        {
            this.Editor = editor;
            this.ModelState = modelState;
        }
    }

    public class AccountEditorFormValidationHandler : CommandValidator<AddOrEditAccountCommand>
    {
        private readonly ISearch<Account> accounts;

        public AccountEditorFormValidationHandler(ISearch<Account> accounts)
        {
            this.accounts = accounts;

            Validators = new Action<AddOrEditAccountCommand>[]
            {
                EnsureSubsidiaryAccountNameIsUnique
            };
        }

        private void EnsureSubsidiaryAccountNameIsUnique(AddOrEditAccountCommand message)
        {
            Logger.Trace("EnsureSubsidiaryAccountNameIsUnique::{0}", message.Editor.Name);

            var isUniqueOrAllowed = !accounts.All()
                .Where(a => a is SubsidiaryAccount)
                .Where(a => a.Id != message.Editor.AccountId)
                .Any(a => a.Name.Equals(message.Editor.Name));

            if (isUniqueOrAllowed)
                return;

            message.ModelState.AddModelError("Name", "The account name ({0}) is already in use."
                .FormatWith(message.Editor.Name));

        }
    }

    public class AddOrEditAccountCommandHandler : IAsyncRequestHandler<AddOrEditAccountCommand, ICommandResult>
    {
        private readonly ISearch<Account> accounts;
        private readonly ISearch<Fund> funds;
        private readonly ILookup<AccountType> accountTypeLookup;
        private readonly IUnitOfWork unitOfWork;
        public ILogger Logger { get; set; }

        public AddOrEditAccountCommandHandler(ISearch<Account> accounts, ISearch<Fund> funds, 
            ILookup<AccountType> accountTypeLookup, IUnitOfWork unitOfWork)
        {
            this.accounts = accounts;
            this.funds = funds;
            this.accountTypeLookup = accountTypeLookup;
            this.unitOfWork = unitOfWork;
        }

        public async Task<ICommandResult> Handle(AddOrEditAccountCommand message)
        {
            Logger.Trace("Handle");

            if (message.ModelState.NotValid())
                return new FailureResult("Validation Failed, please correct the errors below");

            if (message.Editor.AccountId.HasValue)
                return await Edit(message);

            return await Add(message);
        }

        private async Task<ICommandResult> Add(AddOrEditAccountCommand message)
        {
            Logger.Trace("Add");

            var fund = await funds.GetById(message.Editor.ParentFundId ?? -1)
                .OfType<SubsidiaryFund>()
                .FirstOrDefaultAsync();

            var account = message.Editor.BuildAccount(fund, accountTypeLookup);
            fund.Subsidiaries.Add(account);

            await unitOfWork.SaveChangesAsync();

            Logger.Info("Add::Success Id:{0}", account.Id);

            return new SuccessResult(account.Id);
        }

        private async Task<ICommandResult> Edit(AddOrEditAccountCommand message)
        {
            Logger.Trace("Edit::{0}", message.Editor.AccountId);

            var account = await accounts.GetById(message.Editor.AccountId ?? -1)
                .SingleOrDefaultAsync();

            message.Editor.UpdateAccount(account);

            await unitOfWork.SaveChangesAsync();

            Logger.Info("Edit::Success Id:{0}", account.Id);

            return new SuccessResult(account.Id);
        }
    }

    public class AddOrEditAccountCommandFailurePostHandler : IAsyncPostRequestHandler<AddOrEditAccountCommand, ICommandResult>
    {
        public ILogger Logger { get; set; }

        private readonly ISearch<Account> accounts;
        private readonly ISearch<Fund> funds;

        public AddOrEditAccountCommandFailurePostHandler(ISearch<Account> accounts, ISearch<Fund> funds)
        {
            this.accounts = accounts;
            this.funds = funds;
        }

        public async Task Handle(AddOrEditAccountCommand command, ICommandResult response)
        {
            Logger.Trace("Handle");

            if (response.IsSuccess)
                return;

            if (command.Editor.AccountId.HasValue)
            {
                command.Editor.Account = await accounts.GetById(command.Editor.AccountId ?? -1)
                    .SingleOrDefaultAsync();
            }

            if (command.Editor.ParentFundId.HasValue)
            {
                command.Editor.ParentFund = await funds.GetById(command.Editor.ParentFundId ?? -1)
                    .SingleOrDefaultAsync();
            }
        }
    }
}