using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Imports
{
    public class PayRollImportCommand : IAsyncRequest<ICommandResult>
    {
        public PayRollEditorForm Editor { get; private set; }
        public ModelStateDictionary ModelState { get; private set; }

        public PayRollImportCommand(PayRollEditorForm form, ModelStateDictionary modelState)
        {
            this.Editor = form;
            this.ModelState = modelState;
        }
    }

    public class PayRollImportCommandHandler : IAsyncRequestHandler<PayRollImportCommand, ICommandResult>
    {
        public ILogger Logger { get; set; }

        public PayRollImportCommandHandler()
        {
            
        }

        public async Task<ICommandResult> Handle(PayRollImportCommand message)
        {
            Logger.Trace("Handle");

            await Task.FromResult(0);

            if(message.ModelState.NotValid())
                return new FailureResult("Validation Failed, please correct the errors below");

            return new FailureResult("Implement the happy path duh");
        }
    }
}