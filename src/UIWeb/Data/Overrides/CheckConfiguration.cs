using Wiz.Gringotts.UIWeb.Models.Checks;

using System.Data.Entity.ModelConfiguration;


namespace Wiz.Gringotts.UIWeb.Data.Overrides
{
    public class CheckConfiguration: EntityTypeConfiguration<Check>
    {
        public CheckConfiguration()
        {
            Property(a => a.Id)
               .HasColumnName("CheckId");
            HasOptional(c => c.Expense)
                    .WithMany(e => e.Checks)
                    .HasForeignKey(p => new { p.TransactionId, p.AccountId });
            HasOptional(b => b.ExpenseBatch)
                    .WithMany(t => t.Checks)
                    .HasForeignKey(f => new { f.TransactionBatchId });      
        }
    }
}