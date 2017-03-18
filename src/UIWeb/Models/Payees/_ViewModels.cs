using System.Collections.Generic;
using System.Linq;
using Wiz.Gringotts.UIWeb.Models.Clients;
using NExtensions;
using PagedList;

namespace Wiz.Gringotts.UIWeb.Models.Payees
{
    public class PayeeDetails
    {
        public PayeeDetails()
        {
            AttorneyClients = Enumerable.Empty<Client>().ToList();
            GuardianClients = Enumerable.Empty<Client>().ToList();
        }
        public Payee Payee { get; set; }
        public string DisplayAddress
        {
            get
            {
                if (Payee.AddressLine2.HasValue())
                    return "{0}, {1}, {2}, {3}, {4}".FormatWith(Payee.AddressLine1, Payee.AddressLine2, Payee.City, Payee.State, Payee.PostalCode);
                return "{0}, {1}, {2}, {3}".FormatWith(Payee.AddressLine1, Payee.City, Payee.State, Payee.PostalCode);
            }
        }

        public ICollection<Client> AttorneyClients { get; set; }
        public ICollection<Client> GuardianClients { get; set; }
    }

    public class PayeeSearchResult : IPagedSearchResult<Payee>
    {
        public SearchPager Pager { get; set; }
        public IPagedList<Payee> Items { get; set; }

        public string FormatAddress(Payee payee)
        {
            if (payee.AddressLine2.HasValue())
                return "{0}, {1}, {2}, {3}, {4}".FormatWith(payee.AddressLine1, payee.AddressLine2, payee.City, payee.State, payee.PostalCode);
            return "{0}, {1}, {2}, {3}".FormatWith(payee.AddressLine1, payee.City, payee.State, payee.PostalCode);
        }
    }
}