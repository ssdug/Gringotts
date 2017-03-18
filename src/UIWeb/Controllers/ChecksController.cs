using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Filters;
using MediatR;
using Rotativa;
using Wiz.Gringotts.UIWeb.Models.Checks;
using System.Net;

namespace Wiz.Gringotts.UIWeb.Controllers
{
    [Tenant]
    public class ChecksController : Controller
    {
        private readonly IMediator mediator;
        public ILogger Logger { get; set; }

        public ChecksController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<ActionResult> Print(int? id, bool batch = false)
        {
            Logger.Trace("Edit::Get::{0}", id);

            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var query = new CheckEditorFormQuery(Id: id, IsBatchPrint: batch);
            var form = await mediator.SendAsync(query);
            ViewBag.ErrorMessage = "";
            return View(form);
        }

        public async Task<ActionResult> PrintPDF(int? id, bool viewonly = false)
        {
            ViewBag.ViewOnly = viewonly;
            var query = new CheckDetailsQuery(checkId: id.Value);
            var checkDetails = await mediator.SendAsync(query);
            return new ViewAsPdf(checkDetails)
            {
                PageMargins = { Left = 0, Right = 0 }
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Print(CheckEditorForm form, string action)
        {
            int checkId = -1;
            bool viewonly = false;
            Logger.Trace("Create::Print");
            if (action.Equals("Print"))
            {
                if (ModelState.IsValid)
                {
                    var command = new AddOrEditCheckCommand(form, ModelState);
                    var result = await mediator.SendAsync(command);

                    if (result.IsSuccess)
                    {
                        checkId = int.Parse(result.Result.ToString());
                        viewonly = false;
                    }
                    else
                    {
                        return View(form);
                    }
                }
            }
            else if (action.Equals("View"))
            {
                checkId = form.CheckId.Value;
                viewonly = true;
            }
            else
            {
                checkId = -1;
            }

            if (checkId < 0)
            {
                return new HttpNotFoundResult("not implemented");
            }
            return RedirectToAction("PrintPDF", new { Id = checkId, ViewOnly = viewonly });

        }
    }
}