using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Filters;
using Wiz.Gringotts.UIWeb.Models.Imports;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Controllers
{
    [Tenant]
    public class ImportsController : Controller
    {
        private readonly IMediator mediator;
        public ILogger Logger { get; set; }

        public ImportsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult> Create(PayRollEditorForm form)
        {
            Logger.Trace("Create::Post");

            if (ModelState.IsValid)
            {
                var result = await mediator.SendAsync(new PayRollImportCommand(form, ModelState));

                //TODO: handle success result
                if (result.IsSuccess)
                    return HttpNotFound();

                ModelState.AddModelError("", result.Result.ToString());
            }


            //filename to import TODO:  Pass to method for processing
            var postedFile = Request.Files[0];
            
            return View(form);
        } 
    }
}