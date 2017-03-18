using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Filters;
using Wiz.Gringotts.UIWeb.Models;
using Wiz.Gringotts.UIWeb.Models.Clients;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Controllers
{
    [Tenant]
    public class ClientsController : Controller
    {
        private readonly IMediator mediator;
        public ILogger Logger { get; set; }

        public ClientsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<ActionResult> Index(SearchPager searchPager)
        {
            Logger.Trace("Index::{0}", searchPager);

            var query = new ClientSearchQuery(pager: searchPager);
            var result = await mediator.SendAsync(query);

            Logger.Info("{0} matches found", result.Items.TotalItemCount);

            return View(result);
        }

        [ClientContext]
        public async Task<ActionResult> Show(int id)
        {
            Logger.Trace("Show::{0}", id);

            var query = new ClientDetailsQuery(clientId: id);
            var result = await mediator.SendAsync(query);

            if (result == null)
                return RedirectToAction("Index", new { Id = new int?() });

            return View(result);
        }
        
        public async Task<ActionResult> Create()
        {
            Logger.Trace("Create::Get");

            var query = new ClientEditorFormQuery();
            var form = await mediator.SendAsync(query);
            return View(form);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ClientEditorForm form)
        {
            Logger.Trace("Create::Post");

            if (ModelState.IsValid)
            {
                var result = await mediator.SendAsync(new AddOrEditClientCommand(form, ModelState));

                if (result.IsSuccess)
                    return RedirectToAction("Show", new {Id = result.Result});

                ModelState.AddModelError("", result.Result.ToString());
            }

            return View(form);
        }

        [ClientContext]
        public async Task<ActionResult> Edit(int? id)
        {
            Logger.Trace("Edit::Get::{0}", id);

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var query = new ClientEditorFormQuery(clientId: id);
            var form = await mediator.SendAsync(query);
            return View(form);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ClientEditorForm form)
        {
            Logger.Trace("Edit::Post::{0}", form.ClientId);

            if (ModelState.IsValid)
            {
                var result = await mediator.SendAsync(new AddOrEditClientCommand(form, ModelState));

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

            var result = await mediator.SendAsync(new ToggleClientIsActiveCommand(clientId: id));

            if (result.IsSuccess)
                return RedirectToAction("Show", new { Id = id });


            return RedirectToAction("Show", new { Id = id });
           
        } 
    }
}