using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wiz.Gringotts.UIWeb.Models.Clients;
using Wiz.Gringotts.UIWeb.Models.Organizations;

namespace Wiz.Gringotts.UIWeb.Models.LivingUnits
{

    public class LivingUnit : IAmAuditable, IBelongToOrganization, ICanBeActive
    {
        public LivingUnit()
        {
            this.Residencies = new HashSet<Residency>();
        }

        public int Id { get; set; }
        
        public int OrganizationId { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }


        [Required, MaxLength(255), MinLength(1)]
        public string Name { get; set; }

        public virtual ICollection<Residency> Residencies { get; set; }

        public bool IsActive { get; set; }

        public DateTime Created { get; set; }

        [Required, MaxLength(255), MinLength(1)]
        public string CreatedBy { get; set; }

        public DateTime Updated { get; set; }

        [Required, MaxLength(255), MinLength(1)]
        public string UpdatedBy { get; set; }
    }
}