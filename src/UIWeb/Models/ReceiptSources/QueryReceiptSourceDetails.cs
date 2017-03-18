using System.Data.Entity;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.ReceiptSources
{
    public class ReceiptSourceDetailsQuery : IAsyncRequest<ReceiptSourceDetails>
    {
        public int ReceiptSourceId { get; private set; }

        public ReceiptSourceDetailsQuery(int receiptSourceId)
        {
            this.ReceiptSourceId = receiptSourceId;
        }
    }

    public class ReceiptSourceDetailsQueryHandler : IAsyncRequestHandler<ReceiptSourceDetailsQuery, ReceiptSourceDetails>
    {
        private readonly ISearch<ReceiptSource> sources;
        public ILogger Logger { get; set; }

        public ReceiptSourceDetailsQueryHandler(ISearch<ReceiptSource> sources)
        {
            this.sources = sources;
        }

        public async Task<ReceiptSourceDetails> Handle(ReceiptSourceDetailsQuery query)
        {
            Logger.Trace("Handle::{0}", query.ReceiptSourceId);

            var source = await sources.GetById(query.ReceiptSourceId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (source != null)
            {
                return new ReceiptSourceDetails
                {
                    ReceiptSource = source
                };
            }

            return null;
        }
    }
}