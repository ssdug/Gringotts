using System.Data.Entity.ModelConfiguration;
using Wiz.Gringotts.UIWeb.Models.Accounts;

namespace Wiz.Gringotts.UIWeb.Data.Overrides
{
    public class AccountConfiguration : EntityTypeConfiguration<Account>
    {
        public AccountConfiguration()
        {
            Property(a => a.Id)
                .HasColumnName("AccountId");
        }
    }
}