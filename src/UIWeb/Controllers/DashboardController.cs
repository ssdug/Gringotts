using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Filters;
using Wiz.Gringotts.UIWeb.Models.Dashboard;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Controllers
{
    [Tenant]
    public class DashboardController : Controller
    {
        private readonly IMediator mediator;
        public ILogger Logger { get; set; }

        public DashboardController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<ActionResult> Index()
        {
            Logger.Trace("Index");

            var query = new QueryDashboard();
            var result = await mediator.SendAsync(query);

            Logger.Info("{0} widgets found", result.Widgets.Count);

            return View(result);
        }
    }
}