using System.Net;
using System.Web.Mvc;

namespace Wiz.Gringotts.UIWeb
{
    public class HttpForbiddenResult : HttpStatusCodeResult
    {
        public HttpForbiddenResult(): base(HttpStatusCode.Forbidden, "Forbidden")
        { }

        public override void ExecuteResult(ControllerContext context)
        {
            base.ExecuteResult(context);

            var result = new ViewResult
            {
                ViewName = "AccessDenied",
                ViewData = context.Controller.ViewData,
                TempData = context.Controller.TempData
            };

            result.ExecuteResult(context);
        }
    }
}