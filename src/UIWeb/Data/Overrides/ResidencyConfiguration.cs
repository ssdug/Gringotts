using System.Data.Entity.ModelConfiguration;
using Wiz.Gringotts.UIWeb.Models.Clients;

namespace Wiz.Gringotts.UIWeb.Data.Overrides
{
    public class ResidencyConfiguration : EntityTypeConfiguration<Residency>
    {
        public ResidencyConfiguration()
        {
            this.HasOptional(r => r.LivingUnit)
                .WithMany(l => l.Residencies);

            this.HasMany(r => r.Attorneys)
                .WithMany()
                .Map(mc =>
                {
                    mc.MapLeftKey("ResidencyId");
                    mc.MapRightKey("PayeeId");
                    mc.ToTable("ResidencyAttorneys");
                });

            this.HasMany(r => r.Guardians)
                .WithMany()
                .Map(mc =>
                {
                    mc.MapLeftKey("ResidencyId");
                    mc.MapRightKey("PayeeId");
                    mc.ToTable("ResidencyGuardians");
                });
        }
    }
}