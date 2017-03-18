using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Filters;
using Wiz.Gringotts.UIWeb.Models;
using Wiz.Gringotts.UIWeb.Models.ReceiptSources;
using MediatR;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Controllers
{
    [Tenant]
    public class ReceiptSourceController : Controller
    {
        private readonly IMediator mediator;
        public ILogger Logger { get; set; }

        public ReceiptSourceController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<ActionResult> Index(SearchPager searchPager)
        {
            Logger.Trace("Index::{0}", searchPager);

            var query = new ReceiptSourceSearchQuery(pager: searchPager);
            var result = await mediator.SendAsync(query);

            Logger.Info("{0} matches found".FormatWith(result.Items.TotalItemCount));

            return View(result);
        }

        public async Task<ActionResult> Show(int? id)
        {
            Logger.Trace("Index::{0}", id);

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var query = new ReceiptSourceDetailsQuery(receiptSourceId: id.Value);
            var result = await mediator.SendAsync(query);

            if (result == null)
                return new HttpNotFoundResult("An Expense Category with id {0} was not found".FormatWith(id));

            return View(result);
        }

        public async Task<ActionResult> Create()
        {
            Logger.Trace("Create::Get");

            var query = new ReceiptSourceEditorFormQuery();
            var form = await mediator.SendAsync(query);
            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ReceiptSourceEditorForm form)
        {
            Logger.Trace("Create::Post");

            if (ModelState.IsValid)
            {
                var result = await mediator.SendAsync(new AddOrEditReceiptSourceCommand(form, ModelState));

                if (result.IsSuccess)
                    return RedirectToAction("Show", new { Id = result.Result });

                ModelState.AddModelError("", result.Result.ToString());
            }

            return View(form);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            Logger.Trace("Edit::Get::{0}", id);

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var query = new ReceiptSourceEditorFormQuery(receiptSourceId: id);
            var form = await mediator.SendAsync(query);
            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ReceiptSourceEditorForm form)
        {
            Logger.Trace("Edit::Post::{0}", form.ReceiptSourceId);

            if (ModelState.IsValid)
            {
                var result = await mediator.SendAsync(new AddOrEditReceiptSourceCommand(form, ModelState));

                if (result.IsSuccess)
                    return RedirectToAction("Show", new { Id = result.Result });

                ModelState.AddModelError("", result.Result.ToString());
            }
            return View(form);
        }

        public async Task<ActionResult> Toggle(int? id)
        {
            Logger.Trace("Toggle::{0}", id);

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var result = await mediator.SendAsync(new ToggleReceiptSourceIsActiveCommand(receiptSourceId: id.Value));

            if (result.IsSuccess)
                return RedirectToAction("Show", new { Id = id });


            return RedirectToAction("Show", new { Id = id });

        }
    }
}