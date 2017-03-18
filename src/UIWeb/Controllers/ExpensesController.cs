using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Filters;
using Wiz.Gringotts.UIWeb.Models.Transactions;
using MediatR;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Controllers
{
    [Tenant]
    public class ExpensesController : Controller
    {
        private readonly IMediator mediator;
        public ILogger Logger { get; set; }

        public ExpensesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [AccountContext]
        public async Task<ActionResult> Create(int? id, int? orderId = null)
        {
            Logger.Trace("Create::Get::{0}", id);

            var query = new ExpenseEditorFormQuery(accountId: id, orderId: orderId);
            var form = await mediator.SendAsync(query);
            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ExpenseEditorForm form)
        {
            Logger.Trace("Create::Post");

            if (ModelState.IsValid)
            {
                var command = new AddOrEditExpenseCommand(form, ModelState);
                var result = await mediator.SendAsync(command);

                if (result.IsSuccess)
                    return RedirectToAction("Show", "Accounts", new { Id = form.AccountId });

                ModelState.AddModelError("", result.Result.ToString());
            }

            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Void(VoidExpenseEditorForm form)
        {
            Logger.Trace("Void::Post");

            if (ModelState.IsValid)
            {
                var command = new VoidExpenseCommand(form, ModelState);
                var result = await mediator.SendAsync(command);

                //if (result.IsSuccess)
                //    return new RedirectResult(Url.RouteUrl(new {controller = "Accounts", action = "Show", id = form.AccountId}) + "#tab-transactions");

                //ModelState.AddModelError("", result.Result.ToString());
                ModelState.AddModelError("", "You Done Goofed");
            }
            return View(form);
        }

        [BatchContext]
        public async Task<ActionResult> BatchShow(int? id)
        {
            Logger.Trace("Show::{0}", id);

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var query = new BatchExpenseDetailsQuery(batchId: id);
            var result = await mediator.SendAsync(query);

            if (result == null)
                return HttpNotFound();

            return View(result);
        }

        [FundContext]
        public async Task<ActionResult> BatchCreate(int? id)
        {
            Logger.Trace("BatchCreate::Get::{0}", id);

            var query = new BatchExpenseEditorFormQuery(fundId: id);
            var form = await mediator.SendAsync(query);

            if (form == null)
                return new HttpNotFoundResult("An Fund with id {0} was not found".FormatWith(id));

            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BatchCreate(BatchExpenseEditorForm form)
        {
            Logger.Trace("BatchCreate::Post");

            if (ModelState.IsValid)
            {
                var command = new AddOrEditBatchExpenseCommand(form, ModelState);
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

            var query = new BatchExpenseEditorFormQuery(batchId: id);
            var form = await mediator.SendAsync(query);

            if (form == null)
                return new HttpNotFoundResult("An Batch with id {0} was not found".FormatWith(id));

            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BatchEdit(BatchExpenseEditorForm form)
        {
            Logger.Trace("Edit::Post::{0}", form.BatchId);

            if (ModelState.IsValid)
            {
                var command = new AddOrEditBatchExpenseCommand(form, ModelState);
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