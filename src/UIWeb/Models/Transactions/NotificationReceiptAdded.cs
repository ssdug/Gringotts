using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public class ReceiptAddedNotification : IAsyncNotification
    {
        public CommitBatchCommand CommitBatchCommand { get; private set; }
        public AddOrEditReceiptCommand AddOrEditReceiptCommand { get; private set; }
        public Receipt Receipt { get; private set; }

        public ReceiptAddedNotification(Receipt receipt, AddOrEditReceiptCommand addOrEditReceiptCommand = null, CommitBatchCommand commitBatchCommand = null)
        {
            this.CommitBatchCommand = commitBatchCommand;
            this.AddOrEditReceiptCommand = addOrEditReceiptCommand;
            this.Receipt = receipt;
        }
    }
}