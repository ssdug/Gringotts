using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Filters;
using Wiz.Gringotts.UIWeb.Models.Restitution;
using Wiz.Gringotts.UIWeb.Models.Transactions;
using MediatR;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Controllers
{
    [Tenant]
    public class RestitutionController : Controller
    {
        private readonly IMediator mediator;
        public ILogger Logger { get; set; }

        public RestitutionController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [OrderContext]
        public async Task<ActionResult> Show(int id, TransactionsSearchPager searchPager)
        {
            Logger.Trace("Show::{0}", id);

            var query = new RestitutionOrderDetailsQuery(orderId: id, searchPager: searchPager);
            var result = await mediator.SendAsync(query);

            if (result == null)
                return new HttpNotFoundResult("An order with id {0} was not found".FormatWith(id));

            return View(result);
        }


        [ClientContext]
        public async Task<ActionResult> Create(int? id)
        {
            Logger.Trace("Create::Get::{0}", id);

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);


            var query = new RestitutionOrderEditorFormQuery(clientId: id);
            var form = await mediator.SendAsync(query);

            if (form == null)
                return new HttpNotFoundResult("An Client with id {0} was not found".FormatWith(id));

            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RestitutionOrderEditorForm form)
        {
            Logger.Trace("Create::Post");

            if (ModelState.IsValid)
            {
                var result = await mediator.SendAsync(new AddOrEditRestitutionOrderCommand(form, ModelState));

                if (result.IsSuccess)
                    return RedirectToAction("Show", new { Id = result.Result });

                ModelState.AddModelError("", result.Result.ToString());
            }

            return View(form);
        }

        [OrderContext]
        public async Task<ActionResult> Edit(int? id)
        {
            Logger.Trace("Edit::Get::{0}", id);

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var query = new RestitutionOrderEditorFormQuery(orderId: id);
            var form = await mediator.SendAsync(query);
            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RestitutionOrderEditorForm form)
        {
            Logger.Trace("Edit::Post::{0}", form);

            if (ModelState.IsValid)
            {
                var result = await mediator.SendAsync(new AddOrEditRestitutionOrderCommand(form, ModelState));

                if (result.IsSuccess)
                    return RedirectToAction("Show", new { Id = result.Result });

                ModelState.AddModelError("", result.Result.ToString());
            }

            return View(form);
        }
    }
}