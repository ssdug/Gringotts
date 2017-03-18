using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Filters;
using Wiz.Gringotts.UIWeb.Models.Transactions;
using MediatR;
using Rotativa;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Controllers
{
    [Tenant]
    public class ReceiptsController : Controller
    {
        private readonly IMediator mediator;
        public ILogger Logger { get; set; }

        public ReceiptsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [AccountContext]
        public async Task<ActionResult> Create(int? id)
        {
            Logger.Trace("Create::Get::{0}", id);

            var query = new ReceiptEditorFormQuery(accountId: id);
            var form = await mediator.SendAsync(query);
            return View(form);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ReceiptEditorForm form)
        {
            Logger.Trace("Create::Post");

            if (ModelState.IsValid)
            {
                var command = new AddOrEditReceiptCommand(form, ModelState);
                var result = await mediator.SendAsync(command);

                if (result.IsSuccess)
                    return RedirectToAction("Show","Accounts", new { Id = form.AccountId });

                ModelState.AddModelError("", result.Result.ToString());
            }

            return View(form);
        }

        public async Task<ActionResult> Void(int? id)
        {
            Logger.Trace("Void::Get::{0}", id);

            var command = new VoidReceiptCommand(receiptId: id);
            var result = await mediator.SendAsync(command);

            return RedirectToAction("Show", "Accounts", new {id = id});
        }

        [BatchContext]
        public async Task<ActionResult> BatchShow(int? id)
        {
            Logger.Trace("Show::{0}", id);

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var query = new BatchReceiptDetailsQuery(batchId: id);
            var result = await mediator.SendAsync(query);

            if (result == null)
                return HttpNotFound();

            return View(result);
        }


        [BatchContext]
        public async Task<ActionResult> BatchPrint(int? id)
        {
            Logger.Trace("Show::{0}", id);

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var query = new BatchReceiptDetailsQuery(batchId: id);
            var result = await mediator.SendAsync(query);

            if (result == null)
                return HttpNotFound();

            return new ViewAsPdf(result);
        }


        [FundContext]
        public async Task<ActionResult> BatchCreate(int? id)
        {
            Logger.Trace("BatchCreate::Get::{0}", id);

            var query = new BatchReceiptEditorFormQuery(fundId: id);
            var form = await mediator.SendAsync(query);

            if (form == null)
                return new HttpNotFoundResult("An Fund with id {0} was not found".FormatWith(id));

            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BatchCreate(BatchReceiptEditorForm form)
        {
            Logger.Trace("BatchCreate::Post");

            if (ModelState.IsValid)
            {
                var command = new AddOrEditBatchReceiptCommand(form, ModelState);
                var result = await mediator.SendAsync(command);

                if (result.IsSuccess)
                    return RedirectToAction("Show", "Funds", new { Id = form.FundId });

                ModelState.AddModelError("", result.Result.ToString());
            }

            return View(form);
        }

        [BatchContext]
        public async Task<ActionResult> BatchEdit(int? id)
        {
            Logger.Trace("Edit::Get::{0}", id);

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var query = new BatchReceiptEditorFormQuery(batchId: id);
            var form = await mediator.SendAsync(query);

            if (form == null)
                return new HttpNotFoundResult("An Batch with id {0} was not found".FormatWith(id));

            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BatchEdit(BatchReceiptEditorForm form)
        {
            Logger.Trace("Edit::Post::{0}", form.BatchId);

            if (ModelState.IsValid)
            {
                var command = new AddOrEditBatchReceiptCommand(form, ModelState);
                var result = await mediator.SendAsync(command);

                if (result.IsSuccess)
                    return RedirectToAction("Show", "Funds", new { Id = form.FundId });

                ModelState.AddModelError("", result.Result.ToString());
            }

            return View(form);
        }

        public async Task<ActionResult> BatchCommit(int? id)
        {
            Logger.Trace("BatchCommit::{0}", id);

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var result = await mediator.SendAsync(new CommitBatchCommand(batchId: id));

            if (result.IsSuccess)
                return RedirectToAction("BatchShow", new { Id = id });


            return RedirectToAction("BatchShow", new { Id = id });

        }

    }
}