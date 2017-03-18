using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace Wiz.Gringotts.UIWeb.Infrastructure.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string Action<TController>(this UrlHelper helper, Expression<Action<TController>> action)
            where TController : ControllerBase
        {
            var url = LinkBuilder.BuildUrlFromExpression(helper.RequestContext, RouteTable.Routes, action);

            return UrlHelper.GenerateContentUrl("~/" + url, helper.RequestContext.HttpContext);
        }
    }
}