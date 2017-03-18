using Wiz.Gringotts.UIWeb.Models.Organizations;

namespace Wiz.Gringotts.UIWeb.Models
{
    public interface IBelongToOrganization
    {
        int OrganizationId { get; set; }
        Organization Organization { get; set; }
    }
}