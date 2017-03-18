using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using Wiz.Gringotts.UIWeb.Models.Accounts;
using Wiz.Gringotts.UIWeb.Models.LivingUnits;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Wiz.Gringotts.UIWeb.Models.Payees;
using Wiz.Gringotts.UIWeb.Models.Restitution;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Models.Clients
{
    public class Residency : IAmAuditable, IBelongToOrganization, ICanBeActive
    {

        public Residency()
        {
            Created = Updated = DateTime.UtcNow;
            IsActive = true;

            Accounts = new HashSet<ClientAccount>();
            Attorneys = new HashSet<Payee>();
            Guardians = new HashSet<Payee>();
        }
        public int Id { get; set; }

        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }

        public int? LivingUnitId { get; set; }

        [ForeignKey("LivingUnitId")]
        public virtual LivingUnit LivingUnit { get; set; }
        public virtual ICollection<Payee> Attorneys { get; set; }

        public virtual ICollection<Payee> Guardians { get; set; }

        public virtual ICollection<ClientAccount> Accounts { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public int OrganizationId { get; set; }

        [ForeignKey("OrganizationId")]
        public Organization Organization { get; set; }
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class Client : IAmAuditable
    {
        public Client()
        {
            Created = Updated = DateTime.UtcNow;
            Residencies = new List<Residency>();
            Identifiers = new List<ClientIdentifier>();

        }

        public int Id { get; set; }

        public virtual ICollection<Residency> Residencies { get; set; } 

        public virtual ICollection<ClientIdentifier> Identifiers { get; set; }

        [MaxLength(128), MinLength(1)]
        public string FirstName { get; set; }

        [MaxLength(128), MinLength(1)]
        public string MiddleName { get; set; }

        [MaxLength(128), MinLength(1)]
        public string LastName { get; set; }

        public bool HasClientProperty { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        public int? ImageId { get; set; }

        public DateTime Created { get; set; }

        [Required, MaxLength(255), MinLength(1)]
        public string CreatedBy { get; set; }

        public DateTime Updated { get; set; }

        [Required, MaxLength(255), MinLength(1)]
        public string UpdatedBy { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string DisplayName {get; private set;}

        public Residency AddResidency(Organization organization)
        {
            var residency = new Residency {Client = this, OrganizationId = organization.Id, Organization = organization};
            Residencies.Add(residency);
            return residency;
        }
    }

    public class ClientIdentifier : IAmAuditable
    {
        public ClientIdentifier()
        {
            Created = Updated = DateTime.UtcNow;
        }

        public string Display()
        {
            if (ClientIdentifierType.Name == "SSN")
                return Value.SSNMask();

            return Value;
        }

        public int Id { get; set; }

        [Index("IX_ClientIdClientIdentifierTypeId", 1, IsUnique = true)]
        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }

        [Index("IX_ClientIdClientIdentifierTypeId", 2, IsUnique = true)]
        public int ClientIdentiferTypeId { get; set; }

        [ForeignKey("ClientIdentiferTypeId")]
        public virtual ClientIdentifierType ClientIdentifierType { get; set; }

        [Required]
        public string Value { get; set; }

        public DateTime Created { get; set; }

        [Required, MaxLength(255), MinLength(1)]
        public string CreatedBy { get; set; }

        public DateTime Updated { get; set; }

        [Required, MaxLength(255), MinLength(1)]
        public string UpdatedBy { get; set; }
    }

    public class ClientIdentifierType : IAmLookupItem
    {
        public int Id { get; set; }

        [Required, Index(IsUnique = true), 
        MaxLength(255), MinLength(1)]
        public string Name { get; set; }
    }
}