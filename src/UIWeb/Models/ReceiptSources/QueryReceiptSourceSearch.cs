using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.ReceiptSources
{
    public class ReceiptSourceSearchQuery : IAsyncRequest<ReceiptSourceSearchResult>
    {
        public SearchPager Pager { get; private set; }

        public ReceiptSourceSearchQuery(SearchPager pager)
        {
            this.Pager = pager;
        }
    }

    public class ReceiptSourceSearchQueryHandler : IAsyncRequestHandler<ReceiptSourceSearchQuery, ReceiptSourceSearchResult>
    {
        private readonly ISearch<ReceiptSource> sources;
        public ILogger Logger { get; set; }

        public ReceiptSourceSearchQueryHandler(ISearch<ReceiptSource> sources)
        {
            this.sources = sources;
        }

        public async Task<ReceiptSourceSearchResult> Handle(ReceiptSourceSearchQuery message)
        {
            Logger.Trace("Handle");

            var items = sources.GetBySearch(message.Pager)
                .OrderBy(c => c.Name)
                .AsNoTracking();

            return new ReceiptSourceSearchResult
            {
                Pager = message.Pager,
                Items = await items.ToPagedListAsync(message.Pager.Page, message.Pager.PageSize)
            };
        }
    }
}