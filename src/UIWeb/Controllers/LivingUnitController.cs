using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Filters;
using Wiz.Gringotts.UIWeb.Models;
using Wiz.Gringotts.UIWeb.Models.LivingUnits;
using MediatR;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Controllers
{
    [Tenant]
    public class LivingUnitController : Controller
    {
        private readonly IMediator mediator;
        public ILogger Logger { get; set; }

        public LivingUnitController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<ActionResult> Index(SearchPager searchPager)
        {
            Logger.Trace("Index::{0}", searchPager);

            var query = new LivingUnitSearchQuery(pager: searchPager);
            var result = await mediator.SendAsync(query);

            Logger.Info("{0} matches found".FormatWith(result.Items.TotalItemCount));

            return View(result);
        }
    }
}