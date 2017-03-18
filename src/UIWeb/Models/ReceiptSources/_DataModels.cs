using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wiz.Gringotts.UIWeb.Models.Organizations;

namespace Wiz.Gringotts.UIWeb.Models.ReceiptSources
{
    public class ReceiptSource : IBelongToOrganization, IAmAuditable, ICanBeActive
    {
        public ReceiptSource()
        {
            Created = Updated = DateTime.UtcNow;
            IsActive = true;
        }
        public int Id { get; set; }

        [Required, Index("IX_ReceiptSourceNameOrganizationId", 1, IsUnique = true),
        MaxLength(255), MinLength(1)]
        public string Name { get; set; }


        [Index("IX_ReceiptSourceNameOrganizationId", 2, IsUnique = true)]
        public int OrganizationId { get; set; }

        [ForeignKey("OrganizationId")]
        public Organization Organization { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}