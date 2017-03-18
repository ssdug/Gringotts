using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Payees
{
    public class PayeeEditorFormQuery : IAsyncRequest<PayeeEditorForm>
    {
        public int? PayeeId { get; private set; }

        public PayeeEditorFormQuery(int? payeeId = null)
        {
            PayeeId = payeeId;
        }
    }

    public  class PayeeEditorFormQueryHandler : IAsyncRequestHandler<PayeeEditorFormQuery, PayeeEditorForm>
    {
        public ILogger Logger { get; set; }

        private readonly ISearch<Payee> payees;
        private readonly ILookup<PayeeType> payeeTypes;


        public PayeeEditorFormQueryHandler(ISearch<Payee> payees, ILookup<PayeeType> payeeTypes)
        {
            this.payees = payees;
            this.payeeTypes = payeeTypes;
        }

        public async Task<PayeeEditorForm> Handle(PayeeEditorFormQuery message)
        {
            Logger.Trace("Handle::{0}", message.PayeeId);

            var types = GetPayeeTypes();

            if (message.PayeeId.HasValue)
            {
                var payee = await GetPayee(message.PayeeId.Value);
                if(payee != null)
                    return PayeeEditorForm.FromPayee(payee, types);
            }

            return new PayeeEditorForm
            {
                AvailableTypes = types
            };

        }

        private PayeeEditorForm.PayeeType[] GetPayeeTypes()
        {
            Logger.Trace("GetPayeeTypes");

            var result = payeeTypes.All
                .Select(t => new PayeeEditorForm.PayeeType
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToArray();

            if (!result.Any())
            {
                Logger.Warn("no payee types found");
            }

            return result;
        }

        private async Task<Payee> GetPayee(int id)
        {
            Logger.Trace("GetPayee::{0}", id);

            var result = await payees.GetById(id)
                .Include(p => p.Types)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (result == null)
                Logger.Warn("payee {0} not found", id);

            return result;
        }
    }
}