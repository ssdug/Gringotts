using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Wiz.Gringotts.UIWeb.Models.Transactions;

namespace Wiz.Gringotts.UIWeb.Data.Overrides
{
    public class TransactionConfiguration : EntityTypeConfiguration<Transaction>
    {
        public TransactionConfiguration()
        {
            HasKey(a => new {a.Id, a.AccountId});
            
            Property(a => a.Id)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                   .HasColumnName("TransactionId");
        }
    }

    public class TransactionBatchConfiguration : EntityTypeConfiguration<TransactionBatch>
    {
        public TransactionBatchConfiguration()
        {
            Property(b => b.Id)
                .HasColumnName("TransactionBatchId");

            HasOptional(b => b.Trigger)
                .WithMany()
                .HasForeignKey(b => new { b.TriggerId, b.TriggerAccountId });
        }
    }
}