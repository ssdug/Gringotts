using DSHS.WA.LocalFunds.UIWeb.Filters;
using System.Web.Mvc;

namespace DSHS.WA.LocalFunds.UIWeb.Controllers
{
    [Tenant]
    public class ReportsController : Controller
    {     
        public ActionResult ClientsForOrg()
        {
            return View();
        }
        public ActionResult Balance()
        {
            return View();
        }
        public ActionResult BalanceByGivenDay()
        {
            return View();
        }
        public ActionResult DailyBalancing()
        {
            return View();
        }
        public ActionResult Payroll()
        {
            return View();
        }
        public ActionResult ReceiptByStudent()
        {
            return View();
        }
        public ActionResult TransactionByClient()
        {
            return View();
        }
    }
}