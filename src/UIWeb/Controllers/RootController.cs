using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Security;

namespace Wiz.Gringotts.UIWeb.Controllers
{
    public class RootController : Controller
    {
        private readonly IPrincipalProvider principalProvider;
        public ILogger Logger { get; set; }

        public RootController(IPrincipalProvider principalProvider)
        {
            this.principalProvider = principalProvider;
        }

        
        public ActionResult Index()
        {
            Logger.Trace("Index");

            return RedirectToAction("Index", "Dashboard");
        }
        public ActionResult WhoAmI(string username)
        {
            Logger.Trace("WhoAmI::{0}", username);

            ViewBag.Title = string.IsNullOrWhiteSpace(username) ?
                "Who Am I?" : "Who Are They?";

            return View(model: string.IsNullOrWhiteSpace(username) ? 
                principalProvider.GetCurrent().Identity.Name : username);
        }

    }
}