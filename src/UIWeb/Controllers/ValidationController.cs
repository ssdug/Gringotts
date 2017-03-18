using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.ActiveDirectory;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Controllers
{
    [Authorize]
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class ValidationController : Controller
    {
        private readonly IMediator mediator;
        public ILogger Logger { get; set; }

        public ValidationController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<ActionResult> LdapGroup(string groupName, bool allowApplicationGroups = true)
        {
            Logger.Trace("LdapGroup::{0}", groupName);

            var query = new ValidateActiveDirectoryGroupCommand(groupName, allowApplicationGroups);
            var result = await mediator.SendAsync(query);

            return View(result.Result);
        }

        public async Task<ActionResult> LdapUser()
        {
            Logger.Trace("LdapUser");

            var key =
                Request.QueryString.AllKeys.FirstOrDefault(
                    k => k.EndsWith("samaccountname", StringComparison.InvariantCultureIgnoreCase));

            var query = new ValidateActiveDirectoryUserCommand(Request.QueryString[key]);
            var result = await mediator.SendAsync(query);

            return View(result.Result);
        }
    }
}