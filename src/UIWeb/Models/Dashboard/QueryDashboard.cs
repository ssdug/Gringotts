using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Mediation;
using Wiz.Gringotts.UIWeb.Models.Funds;
using Wiz.Gringotts.UIWeb.Models.Imports;
using Wiz.Gringotts.UIWeb.Models.Transactions;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Dashboard
{
    public class QueryDashboard : IAsyncRequest<DashboardResult>
    { }

    public class DashboardQueryHandler : IAsyncRequestHandler<QueryDashboard, DashboardResult>
    {
        public ILogger Logger { get; set; }

        public Task<DashboardResult> Handle(QueryDashboard message)
        {
            Logger.Trace("Handle");

            return Task.FromResult(new DashboardResult());
        }
    }

    public class FundsWidgetPostProcessor : IAsyncPostRequestHandler<QueryDashboard, DashboardResult>
    {
        private readonly ISearch<Fund> funds;
        public ILogger Logger { get; set; }

        public FundsWidgetPostProcessor(ISearch<Fund> funds)
        {
            this.funds = funds;
        }

        public async Task Handle(QueryDashboard command, DashboardResult response)
        {
            Logger.Trace("Handle");

            if (response == null)
                return;

            var items = await funds.All()
                    .ToArrayAsync();

            response.Widgets.Add(new FundsWidget { Funds = items });
        }
    }

    public class BatchWidgetPostProcessor : IAsyncPostRequestHandler<QueryDashboard, DashboardResult>
    {
        private readonly ISearch<TransactionBatch> batches;
        private readonly ISearch<Fund> funds;

        public BatchWidgetPostProcessor(ISearch<TransactionBatch> batches, ISearch<Fund> funds)
        {
            this.batches = batches;
            this.funds = funds;
        }

        public ILogger Logger { get; set; }

        public async Task Handle(QueryDashboard command, DashboardResult response)
        {
            Logger.Trace("Handle");

            if (response == null)
                return;

            var search = new SearchPager {IsActive = true, PageSize = 5, Page = 1};
            var widget = new BatchesWidget
            {
                Batches = await batches.GetBySearch(search)
                    .Take(search.PageSize)
                    .ToArrayAsync(),
                Funds = await funds.All()
                    .ToArrayAsync()
            };

            response.Widgets.Add(widget);
        }
    }

    public class PayRollImportPostProcessor : IAsyncPostRequestHandler<QueryDashboard, DashboardResult>
    {
        public ILogger Logger { get; set; }

        public async Task Handle(QueryDashboard command, DashboardResult response)
        {
            Logger.Trace("Handle");

            if (response == null)
                return;

            response.Widgets.Add(new PayRollImportWidget(new PayRollEditorForm()));

            //TODO: remove this later
            await Task.FromResult(0);
        }
    }
}