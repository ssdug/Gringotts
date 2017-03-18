using System.Collections.Generic;
using Wiz.Gringotts.UIWeb.Models.Funds;
using Wiz.Gringotts.UIWeb.Models.Imports;
using Wiz.Gringotts.UIWeb.Models.Transactions;

namespace Wiz.Gringotts.UIWeb.Models.Dashboard
{
    public class DashboardResult
    {
        public DashboardResult()
        {
            Widgets = new List<Widget>();
        }

        public ICollection<Widget> Widgets { get; set; } 
    }

    public abstract class Widget
    {
        public string Title { get; set; }
        public string ViewName { get; set; }
        public int Order { get; set; }
    }

    public class FundsWidget: Widget
    {
        public FundsWidget()
        {
            Order = 1;
            Title = "Funds";
            ViewName = "FundsWidget";
        }

        public IEnumerable<Fund> Funds { get; set; }
    }

    public class BatchesWidget : Widget
    {
        public BatchesWidget()
        {
            Order = 2;
            Title = "Unconfirmed Batches";
            ViewName = "BatchesWidget";
        }

        public IEnumerable<TransactionBatch> Batches { get; set; }
        public IEnumerable<Fund> Funds { get; set; }
    }

    public class PayRollImportWidget : Widget
    {
        public PayRollEditorForm Form { get; private set; }

        public PayRollImportWidget(PayRollEditorForm form)
        {
            this.Form = form;
            Order = 3;
            Title = "Import PayRoll File";
            ViewName = "PayRollImportWidget";
        }
    }
}