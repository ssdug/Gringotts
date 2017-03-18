using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wiz.Gringotts.UIWeb.Models.Features;

namespace Wiz.Gringotts.UIWeb.Models.Organizations
{
    public class Organization : IAmAuditable
    {
        public Organization()
        {
            Created = Updated = DateTime.UtcNow;
            Children = new List<Organization>();
            Features = new List<Feature>();
        }

        public int Id { get; set; }

        [Required, Index(IsUnique = true), MaxLength(255), MinLength(1)]
        public string Name { get; set; }

        [Required, Index(IsUnique = true), MaxLength(255), MinLength(1)]
        public string GroupName { get; set; }

        [Required, Index(IsUnique = true), MaxLength(255), MinLength(1)]
        public string Abbreviation { get; set; }

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

        [Required, MaxLength(255), MinLength(1)]
        public string FiscalContactSamAccountName { get; set; }

        [Required, MaxLength(255), MinLength(1)]
        public string ITConactSamAccountName { get; set; }

        public virtual Organization Parent { get; set; }
        public virtual ICollection<Organization> Children { get; set; }
        public virtual ICollection<Feature> Features { get; set; }

        public DateTime Created { get; set; }

         [Required, MaxLength(255), MinLength(1)]
        public string CreatedBy { get; set; }

        public DateTime Updated { get; set; }

         [Required, MaxLength(255), MinLength(1)]
        public string UpdatedBy { get; set; }
    }
}