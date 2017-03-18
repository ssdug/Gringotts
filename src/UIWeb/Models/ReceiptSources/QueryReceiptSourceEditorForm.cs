using System.Data.Entity;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.ReceiptSources
{
    public class ReceiptSourceEditorFormQuery : IAsyncRequest<ReceiptSourceEditorForm>
    {
        public int? ReceiptSourceId { get; private set; }

        public ReceiptSourceEditorFormQuery(int? receiptSourceId = null)
        {
            this.ReceiptSourceId = receiptSourceId;
        }
    }

    public class ExpenseCategoryEditorFormQueryHandler : IAsyncRequestHandler<ReceiptSourceEditorFormQuery, ReceiptSourceEditorForm>
    {
        private readonly ISearch<ReceiptSource> categories;
        public ILogger Logger { get; set; }

        public ExpenseCategoryEditorFormQueryHandler(ISearch<ReceiptSource> categories)
        {
            this.categories = categories;
        }

        public async Task<ReceiptSourceEditorForm> Handle(ReceiptSourceEditorFormQuery message)
        {
            Logger.Trace("Handle::{0}", message.ReceiptSourceId);

            if (message.ReceiptSourceId.HasValue)
            {
                var category = await categories.GetById(message.ReceiptSourceId.Value)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (category != null)
                    return ReceiptSourceEditorForm.FromReceiptSource(category);
            }

            return new ReceiptSourceEditorForm();
        }
    }
}