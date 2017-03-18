using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using Wiz.Gringotts.UIWeb.Models;
using Wiz.Gringotts.UIWeb.Models.Accounts;
using Wiz.Gringotts.UIWeb.Models.Clients;
using Wiz.Gringotts.UIWeb.Models.Funds;
using Wiz.Gringotts.UIWeb.Models.Restitution;
using Wiz.Gringotts.UIWeb.Models.Transactions;
using Glimpse.Core.Extensions;

namespace Wiz.Gringotts.UIWeb.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FundContext : ActionFilterAttribute
    {
        public ISearch<Fund> Funds { get; set; }
        public ILogger Logger { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Logger.Trace("OnActionExecuting");

            var fundId = filterContext.ActionParameters["id"].ToString()
                .ToNullableInt32();

            var fund = Funds.GetById(fundId.Value)
                .FirstOrDefault();

            filterContext.Controller.ViewBag.FundContext = fund;

        }
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public class ClientContext : ActionFilterAttribute
    {
        public ISearch<Client> Clients { get; set; }
        public ISearch<Fund> Funds { get; set; } 
        public ILogger Logger { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Logger.Trace("OnActionExecuting");

            var clientId = filterContext.ActionParameters["id"].ToString()
                .ToNullableInt32();

            var client = Clients.GetById(clientId.Value)
                .FirstOrDefault();

            filterContext.Controller.ViewBag.ClientContext = client;

            var fund = Funds.All()
                .FirstOrDefault(f => f.FundType.Code == "651");

            filterContext.Controller.ViewBag.FundContext = fund;

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AccountContext : ActionFilterAttribute
    {
        public ISearch<Account> Accounts { get; set; } 
        public ISearch<Client> Clients { get; set; } 
        public ILogger Logger { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Logger.Trace("OnActionExecuting");

            var accountId = filterContext.ActionParameters["id"].ToString()
                .ToNullableInt32();

            var account = Accounts.GetById(accountId.Value)
                .Include(a => a.Fund)
                .FirstOrDefault();

            if (account == null)
                return;

            filterContext.Controller.ViewBag.AccountContext = account;
            filterContext.Controller.ViewBag.FundContext = account.Fund;

            if (!(account is ClientAccount))
                return;

            var clientId = account.CastOrDefault<ClientAccount>().Residency.ClientId;
            var client = Clients.GetById(clientId)
                .FirstOrDefault();

            filterContext.Controller.ViewBag.ClientContext = client;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class TransactionContext : ActionFilterAttribute
    {
        public ISearch<Models.Transactions.Transaction> Transactions { get; set; }
        public ISearch<Client> Clients { get; set; }
        public ILogger Logger { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Logger.Trace("OnActionExecuting");

            var transactionId = filterContext.ActionParameters["id"].ToString()
                .ToNullableInt32();
            
            var transaction = Transactions.GetById(transactionId.Value)
                .Include(a => a.Account)
                .Include(a => a.Account.Fund)
                .FirstOrDefault();

            if (transaction == null)
                return;

            filterContext.Controller.ViewBag.TransactionContext = transaction;
            filterContext.Controller.ViewBag.AccountContext = transaction.Account;
            filterContext.Controller.ViewBag.FundContext = transaction.Fund;

            if (!(transaction.Account is ClientAccount))
                return;

            var clientId = transaction.Account.CastOrDefault<ClientAccount>().Residency.ClientId;
            var client = Clients.GetById(clientId)
                .FirstOrDefault();

            filterContext.Controller.ViewBag.ClientContext = client;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class OrderContext : ActionFilterAttribute
    {
        public ISearch<Order> Orders { get; set; }
        public ISearch<Fund> Funds { get; set; } 
        public ILogger Logger { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Logger.Trace("OnActionExecuting");
            var orderId = filterContext.ActionParameters["id"].ToString()
                .ToNullableInt32();

            var order = Orders.GetById(orderId.Value)
                .Include(o => o.Residency.Client)
                .FirstOrDefault();

            if (order != null)
            {
                filterContext.Controller.ViewBag.OrderContext = order;
                filterContext.Controller.ViewBag.ClientContext = order.Residency.Client;

                var fund = Funds.All()
                    .FirstOrDefault(f => f.FundType.Code == "651");

                filterContext.Controller.ViewBag.FundContext = fund;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class BatchContext : ActionFilterAttribute
    {
        public ISearch<TransactionBatch> Batches { get; set; } 
        public ILogger Logger { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Logger.Trace("OnActionExecuting");

           var batchId = filterContext.ActionParameters["id"].ToString()
                .ToNullableInt32();

            var batch = Batches.GetById(batchId.Value)
                .Include(b => b.Fund)
                .FirstOrDefault();

            filterContext.Controller.ViewBag.FundContext = batch.Fund;
        }
    }
}