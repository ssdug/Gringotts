using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Models.Payees;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Transactions
{
    public class TrasferAmountEditorFormQuery : IAsyncRequest<TrasferAmountEditorForm>
    {
        public int? FundId { get; private set; }
        public int? ClientId { get; private set; }

        public TrasferAmountEditorFormQuery(int? fundId = null, int? clientId = null)
        {
            this.FundId = fundId;
            this.ClientId = clientId;
        }
    }

    public class TrasferAmountEditorFormQueryHandler : IAsyncRequestHandler<TrasferAmountEditorFormQuery, TrasferAmountEditorForm>
    {
        private readonly ISearch<Payee> payees;
        public ILogger Logger { get; set; }

        public TrasferAmountEditorFormQueryHandler(ISearch<Payee> payees)
        {
            this.payees = payees;
        }

        public async Task<TrasferAmountEditorForm> Handle(TrasferAmountEditorFormQuery query)
        {
            Logger.Trace("Handle::Fund::{0}::Client::{1}", query.FundId, query.ClientId);

            var payee = await payees.All()
                .Where(p => p.Name == "System")
                .Where(p => !p.IsUserSelectable)
                .FirstOrDefaultAsync();

            return new TrasferAmountEditorForm
            {
                FundId = query.FundId,
                ClientId = query.ClientId,
                PayeeId = payee.Id
            };
        }
    }
}