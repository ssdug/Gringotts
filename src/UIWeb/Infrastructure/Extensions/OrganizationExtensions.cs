using System.Collections.Generic;
using Wiz.Gringotts.UIWeb.Models.Organizations;

namespace Wiz.Gringotts.UIWeb.Infrastructure.Extensions
{
    public static class OrganizationExtensions
    {
        public static IEnumerable<Organization> ToHeirarchy(this Organization organization)
        {

            var current = organization;

            if (current.Parent != null)
                foreach (var item in current.Parent.ToHeirarchy())
                    yield return item;

            yield return current;
        } 
    }
}