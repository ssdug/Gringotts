using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Filters;
using Wiz.Gringotts.UIWeb.Models.Accounts;
using Wiz.Gringotts.UIWeb.Models.Transactions;
using MediatR;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Controllers
{
    [Tenant]
    public class AccountsController : Controller
    {
        private readonly IMediator mediator;
        public ILogger Logger { get; set; }

        public AccountsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [AccountContext]
        public async Task<ActionResult> Show(int? id, TransactionsSearchPager searchPager)
        {
            Logger.Trace("Index::{0}", id);

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var query = new AccountDetailsQuery(accountId: id.Value, searchPager: searchPager);
            var result = await mediator.SendAsync(query);

            if (result == null)
                return new HttpNotFoundResult("An Account with id {0} was not found".FormatWith(id));

            return View(result);
        }

        [FundContext]
        public async Task<ActionResult> Create(int? id)
        {
            Logger.Trace("Create::Get::{0}", id);

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var query = new AccountEditorFormQuery(parentFundId: id);
            var form = await mediator.SendAsync(query);

            return View(form);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountEditorForm form)
        {
            Logger.Trace("Create::Post");

            if (ModelState.IsValid)
            {
                var command = new AddOrEditAccountCommand(form, ModelState);
                var result = await mediator.SendAsync(command);

                if (result.IsSuccess)
                    return RedirectToAction("Show", new {Id = result.Result});
            
                ModelState.AddModelError("", result.Result.ToString());
            }

            return View(form);
        }

        [AccountContext]
        public async Task<ActionResult> Edit(int? id)
        {
            Logger.Trace("Edit::Get::{0}", id);

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var query = new AccountEditorFormQuery(accountId: id);
            var form = await mediator.SendAsync(query);

            return View(form);
 
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AccountEditorForm form)
        {
            Logger.Trace("Edit::Post::{0}", form.AccountId);

            if (ModelState.IsValid)
            {
                var command = new AddOrEditAccountCommand(form, ModelState);
                var result = await mediator.SendAsync(command);

                if (result.IsSuccess)
                    return RedirectToAction("Show", new {Id = result.Result});
                
                ModelState.AddModelError("", result.Result.ToString());
            }

            return View(form);
        }

        public async Task<ActionResult> Toggle(int? id)
        {
            Logger.Trace("Toggle::{0}", id);

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var result = await mediator.SendAsync(new ToggleAccountIsActiveCommand(accountId: id));

            if (result.IsSuccess)
                return RedirectToAction("Show", new { Id = id });


            return RedirectToAction("Show", new { Id = id });
        }
        
    }
}