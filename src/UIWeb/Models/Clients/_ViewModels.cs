using Wiz.Gringotts.UIWeb.Models.Accounts;
using Wiz.Gringotts.UIWeb.Models.Restitution;
using Wiz.Gringotts.UIWeb.Models.Users;
using NExtensions;
using PagedList;

namespace Wiz.Gringotts.UIWeb.Models.Clients
{
    public class ClientSearchResult : IPagedSearchResult<Client>
    {
        public SearchPager Pager { get; set; }
        public IPagedList<Client> Items { get; set; }
    }

    public class ClientDetails
    {
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Client.MiddleName))
                    return "{0}, {1}".FormatWith(Client.LastName, Client.FirstName);

                return "{0}, {1} {2}".FormatWith(Client.LastName, Client.FirstName, Client.MiddleName);
            }
        }

        public Client Client { get; set; }

        public Residency Residency { get; set; }

        public ClientAccount[] Accounts { get; set; }
        public Order[] Orders { get; set; }
        public User CreatedBy { get; set; }
        public User UpdatedBy { get; set; }
    }
}