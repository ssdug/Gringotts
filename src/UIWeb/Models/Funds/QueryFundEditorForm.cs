using System.Data.Entity;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Funds
{
    public class FundEditorFormQuery : IAsyncRequest<FundEditorForm>
    {
        public int? FundId { get; private set; }

        public FundEditorFormQuery(int? fundId = null)
        {
            FundId = fundId;
        }
    }

    public class FundEditorFormQueryHandler : IAsyncRequestHandler<FundEditorFormQuery, FundEditorForm>
    {
        private readonly ISearch<Fund> funds;
        public ILogger Logger { get; set; }

        public FundEditorFormQueryHandler(ISearch<Fund> funds)
        {

            this.funds = funds;
        }

        public async Task<FundEditorForm> Handle(FundEditorFormQuery message)
        {
            Logger.Trace("Handle");

            if (message.FundId.HasValue)
            {
                var fund = await funds.GetById(message.FundId.Value)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                return FundEditorForm.FromFund(fund);
            }

            return null;
        }
    }
}