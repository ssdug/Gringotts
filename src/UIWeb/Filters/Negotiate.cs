using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;

namespace Wiz.Gringotts.UIWeb.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class Negotiate : ActionFilterAttribute
    {
        public ILogger Logger { get; set; }

        private readonly IEnumerable<string> pagerFields = new[] { "pager", "items", "page", "pagesize", "search", "enabled" };

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            Logger.Trace("OnActionExecuted");
            var request = filterContext.HttpContext.Request;
            var controller = filterContext.Controller;
            var model = controller.ViewData.Model;

            if (AcceptTypeIsJson(request))
            {
                filterContext.Result = controller.JsonNet(model, GetSerializationFields(request));
            }

            base.OnActionExecuted(filterContext);
        }

        private IEnumerable<string> GetSerializationFields(HttpRequestBase request)
        {
            var itemFields = request.QueryString["fields"];
            return itemFields == null ? pagerFields.Union(new[] { "*" })
                : pagerFields.Union(itemFields.Split(','));
        }

        private bool AcceptTypeIsJson(HttpRequestBase request)
        {
            return request.AcceptTypes != null
                   && request.AcceptTypes.Contains("application/json");
        }
    }
}