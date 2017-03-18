using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.LivingUnits
{
    public class LivingUnitSearchQuery : IAsyncRequest<LivingUnitSearchResult>
    {
        public SearchPager Pager { get; private set; }

        public LivingUnitSearchQuery(SearchPager pager)
        {
            this.Pager = pager;
        }
    }

    public class LivingUnitSearchQueryHandler : IAsyncRequestHandler<LivingUnitSearchQuery, LivingUnitSearchResult>
    {
        public ILogger Logger { get; set; }

        private readonly ISearch<LivingUnit> units;

        public LivingUnitSearchQueryHandler(ISearch<LivingUnit> units)
        {
            this.units = units;
        }

        public async Task<LivingUnitSearchResult> Handle(LivingUnitSearchQuery query)
        {
            Logger.Trace("Handle");

            var items = units.GetBySearch(query.Pager)
                .AsNoTracking()
                .OrderBy(p => p.Name);

            return new LivingUnitSearchResult
            {
                Pager = query.Pager,
                Items = await items.ToPagedListAsync(query.Pager.Page, query.Pager.PageSize)
            };
        }
    }
}