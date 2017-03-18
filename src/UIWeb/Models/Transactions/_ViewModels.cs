using System.Collections.Generic;
using System.Linq;
using Wiz.Gringotts.UIWeb.Models.Users;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public class BatchExpenseDetails
    {
        public ExpenseBatch Batch { get; set; }

        public Expense[] Transactions
        {
            get { return Batch.Transactions.OfType<Expense>().ToArray(); }
        }

        public User CreatedBy { get; set; }
        public User UpdatedBy { get; set; }
    }

    public class BatchReceiptDetails
    {
        public ReceiptBatch Batch { get; set; }

        public Receipt[] Transactions
        {
            get { return Batch.Transactions.OfType<Receipt>().ToArray(); }
        }

        public User CreatedBy { get; set; }
        public User UpdatedBy { get; set; }
    }

    public class BatchTransferDetails
    {
        public TransferBatch Batch { get; set; }

        public Transaction[] Transactions
        {
            get { return Batch.Transactions.ToArray(); }
        }

        public User CreatedBy { get; set; }
        public User UpdatedBy { get; set; }
    }

    public class ReceiptDetails
    {
        public Receipt Receipt { get; set; }
        public string ReceivedBy { get; set; }
    }

 
}