using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Models.Payees
{
    public class Payee : IAmAuditable, IBelongToOrganization, ICanBeActive
    {
        public const string Attorney = "Attorney";
        public const string Guardian = "Guardian";
        public const string Court = "Court";
        public const string Facility = "Facility";

        public Payee()
        {
            Created = Updated = DateTime.UtcNow;
            IsActive = true;
            IsUserSelectable = true;

            Types = new HashSet<PayeeType>();
        }

        public int Id { get; set; }

        [MaxLength(255), MinLength(1)]
        public string Name { get; set; }

        public virtual ICollection<PayeeType> Types { get; set; }

        [DataType(DataType.PhoneNumber), MaxLength(128), MinLength(1)]
        public string Phone { get; set; }

        [Required, MaxLength(255), MinLength(1)]
        public string AddressLine1 { get; set; }

        [MaxLength(255)]
        public string AddressLine2 { get; set; }

        [Required, MaxLength(255), MinLength(1)]
        public string City { get; set; }

        [Required, MaxLength(255), MinLength(1)]
        public string State { get; set; }

        [Required, DataType(DataType.PostalCode), MaxLength(255), MinLength(1)]
        public string PostalCode { get; set; }

        public bool IsUserSelectable { get; set; }

        public string DisplayAddress
        {
            get
            {
                if (AddressLine2.HasValue())
                    return "{0}, {1}, {2}, {3}, {4}".FormatWith(AddressLine1, AddressLine2, City, State, PostalCode);
                return "{0}, {1}, {2}, {3}".FormatWith(AddressLine1, City, State, PostalCode);
            }
        }

        public int OrganizationId { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        public bool IsActive { get; set; }

        public DateTime Created { get; set; }

        [Required, MaxLength(255), MinLength(1)]
        public string CreatedBy { get; set; }

        public DateTime Updated { get; set; }

        [Required, MaxLength(255), MinLength(1)]
        public string UpdatedBy { get; set; }
    }

    public class PayeeType : IAmLookupItem
    {
        public int Id { get; set; }

        [Required, Index(IsUnique = true),
        MaxLength(255), MinLength(1)]
        public string Name { get; set; }

        public virtual ICollection<Payee> Payees { get; set; } 
    }
}