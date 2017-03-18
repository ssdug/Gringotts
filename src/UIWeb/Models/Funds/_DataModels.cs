using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Wiz.Gringotts.UIWeb.Models.Accounts;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Wiz.Gringotts.UIWeb.Models.Transactions;

namespace Wiz.Gringotts.UIWeb.Models.Funds
{
    public class FundType : IAmLookupItem
    {
        public int Id { get; set; }
        
        [Required, Index(IsUnique = true),
        MaxLength(3), MinLength(3)]
        public string Code { get; set; }

        [Required, Index(IsUnique = true),
        MaxLength(255), MinLength(1)]
        public string Name { get; set; }

        public Fund BuildFund(Organization organization)
        {

            if (Code == "651")
                return new ClientFund
                {
                    Code = Code,
                    Name = Name,
                    FundType = this,
                    Organization = organization
                };

            return new SubsidiaryFund
            {
                Code = Code,
                Name = Name,
                FundType = this,
                Organization = organization
            };
        }
    }

    public abstract class Fund : IAmAuditable, IBelongToOrganization, ICanBeActive
    {
        public Fund()
        {
            Transactions = new HashSet<Transaction>();
            Created = Updated = DateTime.UtcNow;
            IsActive = true;
        }

        public int Id { get; set; }

        [MaxLength(255), MinLength(1)]
        public string Name { get; set; }

        [Required,
         MaxLength(3), MinLength(3)]
        public string Code { get; set; }

        [MaxLength(255), MinLength(1)]
        public string BankNumber { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        public int FundTypeId { get; set; }

        [ForeignKey("FundTypeId")]
        public FundType FundType { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }

        public int OrganizationId { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Total { get; set; }
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Encumbered { get; set; }
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Available { get; set; }

        public bool IsActive { get; set; }

        public DateTime Created { get; set; }

        [Required, MaxLength(255), MinLength(1)]
        public string CreatedBy { get; set; }

        public DateTime Updated { get; set; }

        [Required, MaxLength(255), MinLength(1)]
        public string UpdatedBy { get; set; }

        [Timestamp]
        public byte[] Version { get; set; }
    }

    public class SubsidiaryFund : Fund
    {
        public SubsidiaryFund()
        {
            Subsidiaries = new HashSet<SubsidiaryAccount>();
        }
        public virtual ICollection<SubsidiaryAccount> Subsidiaries { get; set; }
    }

    public class ClientFund : Fund
    {
        public ClientFund()
        {
            ClientAccounts = new HashSet<ClientAccount>();
        }
        public virtual ICollection<ClientAccount> ClientAccounts { get; set; }
    }
}