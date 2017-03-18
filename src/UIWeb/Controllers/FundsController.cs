using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Filters;
using Wiz.Gringotts.UIWeb.Models;
using Wiz.Gringotts.UIWeb.Models.Funds;
using MediatR;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Controllers
{
    [Tenant]
    public class FundsController : Controller
    {
        private readonly IMediator mediator;
        public ILogger Logger { get; set; }

        public FundsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<ActionResult> Index(SearchPager searchPager)
        {
            Logger.Trace("Index");

            var query = new FundSearchQuery(pager: searchPager);
            var result = await mediator.SendAsync(query);

            Logger.Info("", result.Items.TotalItemCount);

            return View(result);
        }

        [FundContext]
        public async Task<ActionResult> Show(int? id, SearchPager searchPager)
        {
            Logger.Trace("Index::{0}", id);

            var query = new FundDetailsQuery(fundId: id.Value, searchPager: searchPager);
            var result = await mediator.SendAsync(query);

            if (result == null)
                return new HttpNotFoundResult("A Fund with id {0} was not found".FormatWith(id));

            return View(result);
        }

        [FundContext]
        public async Task<ActionResult> Edit(int? id)
        {
            Logger.Trace("Edit::Get::{0}", id);

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var query = new FundEditorFormQuery(fundId: id);
            var form = await mediator.SendAsync(query);

            return View(form);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(FundEditorForm form)
        {
            Logger.Trace("Edit::Post::{0}", form.FundId);

            if (ModelState.IsValid)
            {
                var command = new AddOrEditFundCommand(form, ModelState);
                var result = await mediator.SendAsync(command);

                if (result.IsSuccess)
                    return RedirectToAction("Show", new { Id = result.Result });

                ModelState.AddModelError("", result.Result.ToString());
            }

            return View(form);
        }
    }
}