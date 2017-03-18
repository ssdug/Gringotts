using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Payees
{
    public class PayeeSearchQuery : IAsyncRequest<PayeeSearchResult>
    {
        public PayeesSearchPager Pager { get; private set; }

        public PayeeSearchQuery(PayeesSearchPager pager)
        {
            this.Pager = pager;
        }
    }

    public class PayeeSearchQueryHandler : IAsyncRequestHandler<PayeeSearchQuery, PayeeSearchResult>
    {
        public ILogger Logger { get; set; }

        private readonly ISearch<Payee> payees;

        public PayeeSearchQueryHandler(ISearch<Payee> payees)
        {
            this.payees = payees;
        }

        public async Task<PayeeSearchResult> Handle(PayeeSearchQuery message)
        {
            Logger.Trace("Handle");

            var items = payees.GetBySearch(message.Pager)
                .AsNoTracking()
                .OrderBy(p => p.Name);

            return new PayeeSearchResult
            {
                Pager = message.Pager,
                Items = await items.ToPagedListAsync(message.Pager.Page, message.Pager.PageSize)
            };
        }
    }
}