using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using MediatR;
using NExtensions;



namespace Wiz.Gringotts.UIWeb.Models.Checks
{
    public class AddOrEditCheckCommand : IAsyncRequest<ICommandResult>
    {
        public CheckEditorForm Editor { get; set; }
        public ModelStateDictionary ModelState { get; set; }

        public AddOrEditCheckCommand(CheckEditorForm editor, ModelStateDictionary modelState)
        {
            Editor = editor;
            ModelState = modelState;
        }
    }

    public class CheckEditorFormValidatorHandler : CommandValidator<AddOrEditCheckCommand>
    {
        private readonly ISearch<Check> checks;
        public CheckEditorFormValidatorHandler(ISearch<Check> checks)
        {
            this.checks = checks;
            Validators = new Action<AddOrEditCheckCommand>[]
            {
                CheckNumberAreUnique
            };
        }
        public void CheckNumberAreUnique(AddOrEditCheckCommand request)
        {
            Logger.Trace("CheckNumberAreUnique");

            var existingCheck = checks.All()
                                .Where(c => c.CheckNumber.ToString() == request.Editor.CheckNumber)
                                .Where(c => c.FundId == request.Editor.FundId);

            if (existingCheck.IsNullOrEmpty())
                return;

            request.ModelState.AddModelError("CheckNumber", "Check number ({0}) is already used".FormatWith(existingCheck.First().CheckNumber));
        }

    }
    public class AddOrEditCheckCommandHandler : IAsyncRequestHandler<AddOrEditCheckCommand, ICommandResult>
    {
        private readonly ApplicationDbContext context;
        public ILogger Logger { get; set; }

        public AddOrEditCheckCommandHandler( ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<ICommandResult> Handle(AddOrEditCheckCommand message)
        {
            Logger.Trace("Handle");

            if (message.ModelState.NotValid())
                return new FailureResult("Validation Failed, please correct the errors below");
            
            return await Add(message);
        }

        private async Task<ICommandResult> Add(AddOrEditCheckCommand message)
        {
            Logger.Trace("Add");
            var check = message.Editor.BuildCheck();

            context.Checks.Add(check);

            await context.SaveChangesAsync();

            Logger.Info("Add::Success Id:{0}", check.Id);

            return new SuccessResult(check.Id);
        }
    }
}