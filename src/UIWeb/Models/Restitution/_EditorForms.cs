using System.ComponentModel.DataAnnotations;
using System.Linq;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Models.Clients;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Wiz.Gringotts.UIWeb.Models.Payees;
using MvcValidationExtensions.Attribute;

namespace Wiz.Gringotts.UIWeb.Models.Restitution
{
    public class RestitutionOrderEditorForm
    {
        public RestitutionOrderEditorForm()
        {
            Withholding = 50;
        }

        public int? ClientId { get; set; }
        public int? OrderId { get; set; }

        [Required]
        [Display(Name = "Court/Cause or Prop Damage #"),
         RegularExpression(InputRegEx.OrderNumber, ErrorMessage = "Invalid Number"),
         MaxLength(255)]
        public string OrderNumber { get; set; }

        public bool IsPropertyDamage { get; set; }

        [Required]
        [Display(Name = "Total Amount of Restitution"),
         DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        [Range(0.01, 1000000.00)]
        [RegularExpression(InputRegEx.Currency,
           ErrorMessage = "Enter a valid money value. 2 Decimals only allowed")]
        public decimal? Amount { get; set; }

        public decimal? Ballance { get; set; }

        [Required]
        [Display(Name = "Payroll Withholding %"),
         Range(1, 99)]
        public int? Withholding { get; set; }

        //typeahead inputs
        [RequiredIf("PayeeName", AllowEmptyStrings = true, ErrorMessage = "Invalid Payee")]
        public int? PayeeId { get; set; }

        [Required]
        public string PayeeName { get; set; }
        //end typeahead inputs

        [Display(Name = "Comments")]
        public string Comments { get; set; }

        public bool IsSatisfied { get; set; }

        [RequiredIfValue("IsSatisfied", "true")]
        public string SatisfiedReason { get; set; }

        public Order BuildOrder(ApplicationDbContext context, Organization organization, Residency residency)
        {
            return new Order
            {
                OrderNumber = this.OrderNumber,
                IsPropertyDamage = this.IsPropertyDamage,
                Total = this.Amount.Value,
                WithholdingPercent = this.Withholding.Value,
                Balance = this.Ballance ?? this.Amount.Value,
                Comments = this.Comments,
                IsSatified = this.IsSatisfied,
                SatisfiedReason = this.SatisfiedReason,
                Payee = GetPayee(context),
                Residency = residency,
                Organization = organization
            };
        }

        private Payee GetPayee(ApplicationDbContext context)
        {
            return context.Payees.First(p => p.Id == this.PayeeId);
        }

        internal static RestitutionOrderEditorForm FromOrder(Order order)
        {
            return new RestitutionOrderEditorForm
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                IsPropertyDamage = order.IsPropertyDamage,
                Amount = order.Total,
                Ballance = order.Balance,
                Withholding = order.WithholdingPercent,
                Comments = order.Comments,
                IsSatisfied = order.IsSatified,
                SatisfiedReason = order.SatisfiedReason,
                PayeeId = order.PayeeId,
                PayeeName = order.Payee.Name
            };
        }

        public void UpdateOrder(Order order, ApplicationDbContext context)
        {
            order.OrderNumber = this.OrderNumber;
            order.IsPropertyDamage = this.IsPropertyDamage;
            order.Total = this.Amount.Value;
            order.WithholdingPercent = this.Withholding.Value;
            order.Comments = this.Comments;
            order.IsSatified = this.IsSatisfied;
            order.SatisfiedReason = this.SatisfiedReason;
            order.Payee = GetPayee(context);
        }
    }

}