using MediatR;

namespace Wiz.Gringotts.UIWeb.Models.Restitution
{
    public class RestitutionOrderAddedOrUpdatedNotification : IAsyncNotification
    {
        public AddOrEditRestitutionOrderCommand Request { get; private set; }
        public Order Order { get; private set; }

        public RestitutionOrderAddedOrUpdatedNotification(AddOrEditRestitutionOrderCommand command, Order order)
        {
            this.Request = command;
            this.Order = order;
        }
    }
}