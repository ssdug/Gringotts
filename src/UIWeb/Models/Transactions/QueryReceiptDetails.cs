using System.Data.Entity;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using MediatR;
using Wiz.Gringotts.UIWeb.Infrastructure.ActiveDirectory;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public class ReceiptDetailsQuery : IAsyncRequest<ReceiptDetails>
    {
        public int TransactionId { get; private set; }

        public ReceiptDetailsQuery(int transactionId)
        {
            this.TransactionId = transactionId;
        }
    }

    public class PrintReceiptQueryHandler : IAsyncRequestHandler<ReceiptDetailsQuery, ReceiptDetails>
    {
        public ILogger Logger { get; set; }

        private readonly ISearch<Receipt> receipts;
        private readonly IUserRepository userRepository;

        public PrintReceiptQueryHandler(ISearch<Receipt> receipts, IUserRepository userRepository)
        {
            this.receipts = receipts;
            this.userRepository = userRepository;
        }


        public async Task<ReceiptDetails> Handle(ReceiptDetailsQuery query)
        {
            Logger.Trace("Handle");

            var transaction = await receipts.GetById(query.TransactionId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (transaction == null)
                return null;

            return new ReceiptDetails
            {
                Receipt = transaction,
                ReceivedBy = userRepository.CurrentUser().DisplayName()
            };
        }
    }
}