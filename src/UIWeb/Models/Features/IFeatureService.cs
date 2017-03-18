using System.Linq;
using Wiz.Gringotts.UIWeb.Infrastructure.Multitenancy;

namespace Wiz.Gringotts.UIWeb.Models.Features
{
    public interface IFeatureService
    {
        bool IsEnabled(string name);
    }

    class FeatureService : IFeatureService
    {
        private readonly FeatureChecker checker;

        public FeatureService(ITenantOrganizationProvider organizationProvider)
        {
            checker = new FeatureChecker(organizationProvider.GetTenantOrganization().Features.ToArray());
        }
        public bool IsEnabled(string name)
        {
            return checker.IsEnabled(name);
        }
    }
}