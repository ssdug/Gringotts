using Wiz.Gringotts.UIWeb.Models.Organizations;
using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Features
{
    public class FeatureDisabledNotification : IAsyncNotification
    {
        public Organization Organization { get; set; }
        public Feature Feature { get; set; }

        public FeatureDisabledNotification(Organization organization, Feature feature)
        {
            Organization = organization;
            Feature = feature;
        }
    }
}