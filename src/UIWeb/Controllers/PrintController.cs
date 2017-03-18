using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Filters;
using Wiz.Gringotts.UIWeb.Models.Transactions;
using MediatR;
using NExtensions;
using Rotativa;
using Wiz.Gringotts.UIWeb.Models.Checks;

namespace Wiz.Gringotts.UIWeb.Controllers
{
    [Tenant]
    public class PrintController : Controller
    {
        private readonly IMediator mediator;
        public ILogger Logger { get; set; }

        public PrintController(IMediator mediator)
        {
            this.mediator = mediator;
        }
  
        public async Task<ActionResult> PrintReceipt(int id)
        {
            Logger.Trace("PrintReceipt::{0}", id);

            var query = new ReceiptDetailsQuery(transactionId: id);
            var result = await mediator.SendAsync(query);

            if (result == null)
                return new HttpNotFoundResult("A Receipt with id {0} was not found".FormatWith(id));


            return new ViewAsPdf(result);
        }
    }
}