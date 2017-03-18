using System.Threading.Tasks;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using MediatR;
using WebGrease;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public class VoidReceiptCommand : IAsyncRequest<ICommandResult>
    {
        public int? ReceiptId { get; private set; }

        public VoidReceiptCommand(int? receiptId)
        {
            this.ReceiptId = receiptId;
        }
    }


    public class VoidReceiptCommandHandler : IAsyncRequestHandler<VoidReceiptCommand, ICommandResult>
    {
        public async Task<ICommandResult> Handle(VoidReceiptCommand message)
        {
            return await Task.FromResult(new SuccessResult(true));
        }
    }
}