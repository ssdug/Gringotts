using System.Web.Mvc;
using Autofac.Extras.NLog;
using Microsoft.Ajax.Utilities;

namespace Wiz.Gringotts.UIWeb.Filters
{
    public class ReturnUrl : ActionFilterAttribute
    {
        public ILogger Logger { get; set; }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            Logger.Trace("OnActionExecuted");

            if (filterContext.HttpContext.Request.HttpMethod != "POST")
                return;

            var url = filterContext.HttpContext.Request.Form["return_url"];

            if (url.IsNullOrWhiteSpace())
                return;

            if (filterContext.Result is RedirectResult || filterContext.Result is RedirectToRouteResult)
            {
                filterContext.Result = new RedirectResult(url);
            }
        }
    }
}