using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Filters;
using Wiz.Gringotts.UIWeb.Models.Transactions;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Controllers
{
    [Tenant]
    public class TransfersController : Controller
    {
        private readonly IMediator mediator;
        public ILogger Logger { get; set; }

        public TransfersController(IMediator  mediator)
        {
            this.mediator = mediator;
        }

        [FundContext]
        public async Task<ActionResult> Fund(int? id)
        {
            Logger.Trace("Fund::Get::{0}", id);

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var query = new TrasferAmountEditorFormQuery(fundId: id);
            var form = await mediator.SendAsync(query);

            return View("Create",form);
        }

        [ClientContext]
        public async Task<ActionResult> Client(int? id)
        {
            Logger.Trace("Client::Get::{0}", id);

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var query = new TrasferAmountEditorFormQuery(clientId: id);
            var form = await mediator.SendAsync(query);

            return View("Create", form);
        }

        [BatchContext]
        public async Task<ActionResult> BatchShow(int? id)
        {
            Logger.Trace("Show::{0}", id);

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var query = new BatchTransferDetailsQuery(batchId: id);
            var result = await mediator.SendAsync(query);

            if (result == null)
                return HttpNotFound();

            return View(result);
        }

        public async Task<ActionResult> Create(TrasferAmountEditorForm form)
        {
            Logger.Trace("Create::Post");

            if (ModelState.IsValid)
            {
                var result = await mediator.SendAsync(new TransferAmountCommand(form, ModelState));

                if (result.IsSuccess)
                    return RedirectToAction("BatchShow", new { Id = result.Result });

                ModelState.AddModelError("", result.Result.ToString());
            }

            return View(form);
        }
    }
}