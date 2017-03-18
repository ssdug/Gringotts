using System.Data.Entity.ModelConfiguration;
using Wiz.Gringotts.UIWeb.Models.Organizations;

namespace Wiz.Gringotts.UIWeb.Data.Overrides
{
    public class OrganizationConfiguration : EntityTypeConfiguration<Organization>
    {
        public OrganizationConfiguration()
        {
            
            this.HasMany(o => o.Features)
                .WithMany()
                .Map(mo =>
                {
                    mo.MapLeftKey("OrganizationId");
                    mo.MapRightKey("FeatureId");
                    mo.ToTable("OrganizationFeatures");
                });
        }
    }
}