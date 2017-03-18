using System.Collections.Generic;
using System.Linq;

namespace Wiz.Gringotts.UIWeb.Models.Features
{
    public class FeatureChecker
    {
        private readonly IEnumerable<Feature> enabledFeatures;

        public FeatureChecker(IEnumerable<Feature> enabledFeatures)
        {
            this.enabledFeatures = enabledFeatures;
        }

        public bool IsEnabled(string name)
        {
            return enabledFeatures.Any(f => f.Name == name);
        }
    }
}