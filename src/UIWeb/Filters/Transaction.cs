using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;

namespace Wiz.Gringotts.UIWeb.Filters
{

    //TODO: this transaction filter causes NAN display in 
    //glimpse if a command is not issued in the transaction.
    //this bug is fixed in version 1.5.4 when it is released.
    public class Transaction : ActionFilterAttribute
    {
        private readonly IUnitOfWork unitOfWork;
        public ILogger Logger { get; set; }


        public Transaction(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Logger.Trace("OnActionExecuting::Transaction Started");
            unitOfWork.BeginTransaction();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            Logger.Trace("OnActionExecuted::Transaction Closing");
            unitOfWork.CloseTransaction(filterContext.Exception);
            Logger.Trace("OnActionExecuted::Transaction Closed");

        }
    }
}