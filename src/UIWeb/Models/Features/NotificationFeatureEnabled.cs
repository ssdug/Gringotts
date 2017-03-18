using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using MediatR;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Models.Features
{
    public class FeatureEnabledNotification : IAsyncNotification
    {
        public Organization Organization { get; set; }
        public Feature Feature { get; set; }

        public FeatureEnabledNotification(Organization organization, Feature feature)
        {
            Organization = organization;
            Feature = feature;
        }
    }

    public class FundsFeatureEnabledNotificationHandler : IAsyncNotificationHandler<FeatureEnabledNotification>
    {
        public ILogger Logger { get; set; }

        private readonly ApplicationDbContext context;

        public FundsFeatureEnabledNotificationHandler(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task Handle(FeatureEnabledNotification notification)
        {
            Logger.Trace("Handle");

            if (!notification.Feature.Name.Equals(Feature.Funds))
                return;

            var existingFundTypes = await context.Funds
                .Where(f => f.Organization.Id == notification.Organization.Id)
                .Select(f => f.FundType.Id)
                .ToArrayAsync();

            Logger.Info("{0} existing funds found", existingFundTypes.Length);

            var newFundTypes = await context.FundTypes
                .Where(t => !existingFundTypes.Contains(t.Id))
                .ToArrayAsync();

            Logger.Info("Creating {0} new funds", newFundTypes.Length);

            newFundTypes.ForEach(fundType =>
            {
                context.Funds.Add(fundType.BuildFund(notification.Organization));
                Logger.Info("Created {0} ({1}) for organization {2}",
                    fundType.Name, fundType.Code, notification.Organization.Name);
            });

            await context.SaveChangesAsync();
            Logger.Info("Success:: {0} new funds created.", newFundTypes.Length);
        }
    }
}