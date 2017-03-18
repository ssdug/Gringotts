using System.Linq;
using System.Web.Mvc;
using Autofac.Extras.NLog;

namespace Wiz.Gringotts.UIWeb.Filters
{
    /// <summary>
    /// Filters tenant key out of non tenant routes so when a user
    /// navigates from a tenant specific area to a non tenant specific area
    /// the tenant key is removed from the url.
    /// </summary>
    public class NonTenant : ActionFilterAttribute
    {
        public ILogger Logger { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Logger.Trace("OnActionExecuting");

            if (!filterContext.RouteData.Values.ContainsKey("tenant"))
                return;

            if (ControllerIsTenantController(filterContext.Controller))
                return;

            filterContext.RouteData.Values.Remove("tenant");

            filterContext.Result = new RedirectToRouteResult("Default",
                filterContext.RouteData.Values);
        }

        private static bool ControllerIsTenantController(ControllerBase controller)
        {
            return controller.GetType()
                .GetCustomAttributes(typeof (Tenant), true)
                .Any();
        }
    }
}