using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using MediatR;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Models.Payees
{
    public class AddOrEditPayeeCommand : IAsyncRequest<ICommandResult>
    {
        public PayeeEditorForm Editor { get; private set; }
        public ModelStateDictionary ModelState { get; private set; }

        public AddOrEditPayeeCommand(PayeeEditorForm editor, ModelStateDictionary modelState)
        {
            Editor = editor;
            ModelState = modelState;
        }
    }

    public class PayeeEditorFormPayeeTypeValidatorHandler : CommandValidator<AddOrEditPayeeCommand>
    {
        public PayeeEditorFormPayeeTypeValidatorHandler()
        {
            Validators = new Action<AddOrEditPayeeCommand>[]
            {
                EnsurePayeeTypesAreDistinct
            };
        }

        public void EnsurePayeeTypesAreDistinct(AddOrEditPayeeCommand request)
        {
            Logger.Trace("EnsurePayeeTypesAreDistinct");

            var duplicatesTypeIds = (from item in request.Editor.SelectedTypes
                group item by item.Id into grp
                where grp.Count() > 1
                select grp.Key)
                .ToArray();

            var seenTypeIds = new List<int>();

            if (!duplicatesTypeIds.Any())
                return;

            for (var i = 0; i < request.Editor.SelectedTypes.Count; i++)
            {
                var selectedType = request.Editor.SelectedTypes[i];

                if (duplicatesTypeIds.Any(d => d == selectedType.Id) && seenTypeIds.Contains(selectedType.Id))
                    request.ModelState.AddModelError("SelectedTypes[{0}].Id".FormatWith(i), "Duplicate Payee Type not allowed");

                seenTypeIds.Add(selectedType.Id);
            }
        }
    }

    public class AddOrEditPayeeCommandHandler : IAsyncRequestHandler<AddOrEditPayeeCommand, ICommandResult>
    {
        public ILogger Logger { get; set; }

        private readonly ISearch<Payee> payees;
        private readonly ApplicationDbContext context;

        public AddOrEditPayeeCommandHandler(ISearch<Payee> payees, ApplicationDbContext context)
        {
            this.payees = payees;
            this.context = context;
        }

        public async Task<ICommandResult> Handle(AddOrEditPayeeCommand message)
        {
            Logger.Trace("Handle");

            if (message.ModelState.NotValid())
                return new FailureResult("Validation Failed, please correct the errors below");

            if (message.Editor.PayeeId.HasValue)
                return await Edit(message);

            return await Add(message);
        }

        private async Task<ICommandResult> Add(AddOrEditPayeeCommand message)
        {
            Logger.Trace("Add");

            var payee = message.Editor.BuildPayee(context);
            context.Payees.Add(payee);

            await context.SaveChangesAsync();

            Logger.Info("Add::Success Id:{0}", payee.Id);

            return new SuccessResult(payee.Id);
        }

        private async Task<ICommandResult> Edit(AddOrEditPayeeCommand message)
        {
            Logger.Trace("Edit::{0}", message.Editor.PayeeId);

            var payee = await payees.GetById(message.Editor.PayeeId.Value)
                .SingleAsync();

            message.Editor.UpdatePayee(payee, context);

            await context.SaveChangesAsync();

            Logger.Info("Edit::Success Id:{0}", payee.Id);

            return new SuccessResult(payee.Id);
        }
    }
}