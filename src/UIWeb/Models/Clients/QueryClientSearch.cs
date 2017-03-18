using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Clients
{
    public class ClientSearchQuery : IAsyncRequest<ClientSearchResult>
    {
        public SearchPager Pager { get; private set; }

        public ClientSearchQuery(SearchPager pager)
        {
            this.Pager = pager;
        }
    }

    public class ClientSearchQueryHandler : IAsyncRequestHandler<ClientSearchQuery,ClientSearchResult>
    {
        public ILogger Logger { get; set; }

        private readonly ISearch<Client> clients;

        public ClientSearchQueryHandler(ISearch<Client> clients)
        {
            this.clients = clients;
        }

        public async Task<ClientSearchResult> Handle(ClientSearchQuery query)
        {
            Logger.Trace("Handle");

            var items = clients.GetBySearch(query.Pager)
                                .Include(c => c.Residencies)
                                .Include(c => c.Identifiers)
                                .Include("Identifiers.ClientIdentifierType")
                                .OrderBy(c => c.LastName)
                                .AsNoTracking();

            return new ClientSearchResult
            {
                Pager = query.Pager,
                Items = await items.ToPagedListAsync(query.Pager.Page, query.Pager.PageSize)
            };

        }
    }
}