using System.Data.Entity.ModelConfiguration;
using Wiz.Gringotts.UIWeb.Models.Funds;

namespace Wiz.Gringotts.UIWeb.Data.Overrides
{
    public class SubsidiaryFundConfiguration : EntityTypeConfiguration<SubsidiaryFund>
    {
        public SubsidiaryFundConfiguration()
        {
            this.HasMany(s => s.Subsidiaries)
                .WithMany()
                .Map(mc =>
                {
                    mc.MapLeftKey("FundId");
                    mc.MapRightKey("AccountId");
                    mc.ToTable("FundsSubsidiaryAccounts");
                });
        }
    }

    public class ClientFundConfiguration : EntityTypeConfiguration<ClientFund>
    {
        public ClientFundConfiguration()
        {
            this.HasMany(c => c.ClientAccounts)
                .WithMany()
                .Map(mc =>
                {
                    mc.MapLeftKey("FundId");
                    mc.MapRightKey("AccountId");
                    mc.ToTable("FundsClientAccounts");
                });
        }
    }
}