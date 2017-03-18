using System;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Filters
{
    [TestClass]
    public class TransactionTests
    {
        private IUnitOfWork coordinator;
        private Transaction filter;

        [TestInitialize]
        public void Init()
        {
            coordinator = Substitute.For<IUnitOfWork>();

   
            filter = new Transaction(coordinator)
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod]
        public void Should_being_transaction_on_executing()
        {
            filter.OnActionExecuting(null);

            coordinator.Received().BeginTransaction();
        }

        [TestMethod]
        public void Should_close_transaction_on_executed()
        {
            var exception = new Exception();
            var filterContext = new ActionExecutedContext
            {
                Exception = exception
            };
            filter.OnActionExecuted(filterContext);

            coordinator.Received().CloseTransaction(exception);
        }
    }
}
