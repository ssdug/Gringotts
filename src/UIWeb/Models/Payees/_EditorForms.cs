using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Models.Payees
{
    public class PayeeEditorForm
    {
        public PayeeEditorForm()
        {
            AvailableTypes = new List<PayeeType>();
            SelectedTypes = new List<PayeeType>();
        }

        public int? PayeeId { get; set; }

        [Display(Name = "Name"), Required,
         MaxLength(255), MinLength(1)]
        public string Name { get; set; }

        [DataType(DataType.PhoneNumber),
         RegularExpression(InputRegEx.PhoneNumber, ErrorMessage = "Invalid Phone Number"),
         MaxLength(128), MinLength(1)]
        public string Phone { get; set; }

        [Required, Display(Name = "Line 1"), MaxLength(255), MinLength(1)]
        public string AddressLine1 { get; set; }

        [Display(Name = "Line 2"), MaxLength(255)]
        public string AddressLine2 { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required"), MaxLength(255), MinLength(1)]
        public string City { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required"),
         MaxLength(2, ErrorMessage = "Invalid State"),
         MinLength(2, ErrorMessage = "Invalid State")]
        public string State { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required"),
         Display(Name = "Postal Code"),
         DataType(DataType.PostalCode),
         RegularExpression(InputRegEx.PostalCode, ErrorMessage = "Invalid Postal Code"),
         MaxLength(255), MinLength(1)]
        public string PostalCode { get; set; }

        public IList<PayeeType> AvailableTypes { get; set; }

        public IList<PayeeType> SelectedTypes { get; set; }

        public Payee BuildPayee(ApplicationDbContext context)
        {
            return new Payee
            {
                Name = this.Name.TrimmedOrNull(),
                Phone = this.Phone.TrimmedOrNull(),
                AddressLine1 = this.AddressLine1.TrimmedOrNull(),
                AddressLine2 = this.AddressLine2.TrimmedOrNull(),
                City = this.City.TrimmedOrNull(),
                State = this.State == null ? null : this.State.ToUpper().TrimmedOrNull(),
                PostalCode = this.PostalCode.TrimmedOrNull(),
                Types = GetPayeeTypes(context)
            };
        }

        private ICollection<Payees.PayeeType> GetPayeeTypes(ApplicationDbContext context)
        {
            var typeIds = this.SelectedTypes.Select(t => t.Id).ToArray();

            return context.PayeeTypes
                .Where(t => typeIds.Contains(t.Id))
                .ToList();
        }

        public void UpdatePayee(Payee payee, ApplicationDbContext context)
        {
            payee.Name = this.Name.TrimmedOrNull();
            payee.Phone = this.Phone.TrimmedOrNull();
            payee.AddressLine1 = this.AddressLine1.TrimmedOrNull();
            payee.AddressLine2 = this.AddressLine2.TrimmedOrNull();
            payee.City = this.City.TrimmedOrNull();
            payee.State = this.State == null ? null : this.State.ToUpper().TrimmedOrNull();
            payee.PostalCode = this.PostalCode.TrimmedOrNull();

            //remove deleted payee types
            payee.Types.Where(t => this.SelectedTypes.All(o => o.Id != t.Id))
                .ToArray()
                .ForEach(t =>
                {
                    payee.Types.Remove(t);
                });

            //add new payee types
            this.SelectedTypes.Where(t => payee.Types.All(i => i.Id != t.Id))
                .ForEach(i =>
                {
                    payee.Types.Add(context.PayeeTypes.First(t => t.Id == i.Id));
                });
        }

        public static PayeeEditorForm FromPayee(Payee payee, PayeeType[] payeeTypes)
        {
            return new PayeeEditorForm
            {
                PayeeId = payee.Id,
                Name = payee.Name,
                Phone = payee.Phone,
                AddressLine1 = payee.AddressLine1,
                AddressLine2 = payee.AddressLine2,
                City = payee.City,
                State = payee.State,
                PostalCode = payee.PostalCode,
                SelectedTypes = payee.Types
                    .Select(t => new PayeeType
                    {
                        Id = t.Id,
                        Name = t.Name
                    }).ToArray(),
                AvailableTypes = payeeTypes
            };
        }

        public class PayeeType
        {
            [Display(Name = "Payee Type")]
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}