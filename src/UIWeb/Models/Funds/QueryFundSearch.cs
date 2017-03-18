using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Funds
{
    public class FundSearchQuery : IAsyncRequest<FundsSearchResult>
    {
        public SearchPager Pager { get; private set; }

        public FundSearchQuery(SearchPager pager)
        {
            this.Pager = pager;
        }
    }

    public class FundsSearchQueryHandler : IAsyncRequestHandler<FundSearchQuery, FundsSearchResult>
    {
        private readonly ISearch<Fund> funds;

        public ILogger Logger { get; set; }

        public FundsSearchQueryHandler(ISearch<Fund> funds)
        {
            this.funds = funds;
        }

        public async Task<FundsSearchResult> Handle(FundSearchQuery query)
        {
            Logger.Trace("Handle");

            var items = funds.GetBySearch(query.Pager)
                .Include(f => f.FundType)
                .OrderBy(f => f.Code)
                .AsNoTracking();

            return new FundsSearchResult
            {
                Pager = query.Pager,
                Items = await items.ToPagedListAsync(query.Pager.Page, query.Pager.PageSize)
            };
        }
    }
}