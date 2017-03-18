using System.Data.Entity.ModelConfiguration;
using Wiz.Gringotts.UIWeb.Models.Transactions;

namespace Wiz.Gringotts.UIWeb.Data.Overrides
{
    public class TransactionSubTypeConfiguration : EntityTypeConfiguration<TransactionSubType>
    {
        public TransactionSubTypeConfiguration()
        {
            Property(a => a.Id)
                .HasColumnName("TransactionSubTypeId");
        }
    }
}