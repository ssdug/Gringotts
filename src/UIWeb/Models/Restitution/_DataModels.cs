using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wiz.Gringotts.UIWeb.Models.Clients;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Wiz.Gringotts.UIWeb.Models.Payees;

namespace Wiz.Gringotts.UIWeb.Models.Restitution
{
    public class Order : IBelongToOrganization, IAmAuditable
    {
        public Order()
        {
            Created = Updated = DateTime.UtcNow;
        }

        public int Id { get; set; }

        [MaxLength(128), MinLength(1)]
        public string OrderNumber { get; set; }

        public bool IsPropertyDamage { get; set; }

        [Range(typeof(int), "0", "100")]
        public int WithholdingPercent { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Total { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Balance { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        public bool IsSatified { get; set; }

        [DataType(DataType.MultilineText)]
        public string SatisfiedReason { get; set; }

        public int PayeeId { get; set; }

        [ForeignKey("PayeeId")]
        public virtual Payee Payee { get; set; }

        public int ResidencyId { get; set; }

        [ForeignKey("ResidencyId")]
        public virtual Residency Residency { get; set; }

        public int OrganizationId { get; set; }

        [ForeignKey("OrganizationId")]
        public Organization Organization { get; set; }

        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
    }
}