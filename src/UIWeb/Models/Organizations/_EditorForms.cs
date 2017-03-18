using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using Wiz.Gringotts.UIWeb.Models.Features;

namespace Wiz.Gringotts.UIWeb.Models.Organizations
{
    public class OrganizationEditorForm
    {
        public OrganizationEditorForm()
        {
            EnabledFeatures = new List<string>();
            AvailableFeatures = new List<Feature>();
        }

        //parent organization details
        public string ParentOrganizationAbbreviation { get; set; }
        public string ParentOrganizationName { get; set; }

        public int? ParentOrganizationId { get; set; }


        //organization details
        public int? OrganizationId { get; set; }

        [Required, Display(Name = "AD Group"), Remote("LdapGroup", "Validation", AdditionalFields = "AllowApplicationGroups")]
        public string GroupName { get; set; }

        public bool AllowApplicationGroups { get { return false; } }

        [Required, MaxLength(255), MinLength(1)]
        public string Name { get; set; }

        [Required,
         RegularExpression(InputRegEx.Abbreviation, ErrorMessage = "Abbreviations must have no spaces and contain only letters"),
         MaxLength(255), MinLength(1)]
        public string Abbreviation { get; set; }

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

        [Required, Display(Name = "Fiscal Contact"),
         Remote("LdapUser", "Validation"),
         MaxLength(255), MinLength(1)]
        public string FiscalContactSamAccountName { get; set; }

        [Required, Display(Name = "IT Conact"),
         Remote("LdapUser", "Validation"),
         MaxLength(255), MinLength(1)]
        public string ITConactSamAccountName { get; set; }

        //organization features
        public ICollection<string> EnabledFeatures { get; set; }

        public IList<Feature> AvailableFeatures { get; set; }


        public static OrganizationEditorForm FromOrganization(Organization organization, Feature[] availableFeatures)
        {
            return new OrganizationEditorForm
            {
                OrganizationId = organization.Id,
                Name = organization.Name,
                GroupName = organization.GroupName,
                Abbreviation = organization.Abbreviation,
                Phone = organization.Phone,
                AddressLine1 = organization.AddressLine1,
                AddressLine2 = organization.AddressLine2,
                City = organization.City,
                State = organization.State,
                PostalCode = organization.PostalCode,

                FiscalContactSamAccountName = organization.FiscalContactSamAccountName,
                ITConactSamAccountName = organization.ITConactSamAccountName,


                ParentOrganizationAbbreviation = organization.Parent != null ? organization.Parent.Abbreviation : null,
                ParentOrganizationName = organization.Parent != null ? organization.Parent.Name : null,
                ParentOrganizationId = organization.Parent != null
                                         ? organization.Parent.Id
                                         : new int?(),

                AvailableFeatures = availableFeatures,
                EnabledFeatures = organization.Features.Any()
                                    ? organization.Features.Select(f => f.Id.ToString()).ToList()
                                    : new List<string>()
            };
        }

        public static OrganizationEditorForm FromParentOrganization(Organization parent, Feature[] availableFeatures)
        {
            return new OrganizationEditorForm
            {
                ParentOrganizationId = parent.Id,
                ParentOrganizationAbbreviation = parent.Abbreviation,
                ParentOrganizationName = parent.Name,
                AvailableFeatures = availableFeatures
            };
        }

        public void UpdateOrganization(Organization organization)
        {
            organization.Name = this.Name.TrimmedOrNull();
            organization.GroupName = this.GroupName.TrimmedOrNull();
            organization.Abbreviation = this.Abbreviation.TrimmedOrNull();
            organization.Phone = this.Phone.TrimmedOrNull();
            organization.AddressLine1 = this.AddressLine1.TrimmedOrNull();
            organization.AddressLine2 = this.AddressLine2.TrimmedOrNull();
            organization.City = this.City.TrimmedOrNull();
            organization.State = this.State == null ? null : this.State.ToUpper().TrimmedOrNull();
            organization.PostalCode = this.PostalCode.TrimmedOrNull();

            organization.FiscalContactSamAccountName = this.FiscalContactSamAccountName.TrimmedOrNull();
            organization.ITConactSamAccountName = this.ITConactSamAccountName.TrimmedOrNull();
        }

        public Organization BuildOrganiation(ApplicationDbContext context)
        {
            return new Organization
            {
                Name = this.Name.TrimmedOrNull(),
                GroupName = this.GroupName.TrimmedOrNull(),
                Abbreviation = this.Abbreviation.TrimmedOrNull(),
                Phone = this.Phone.TrimmedOrNull(),
                AddressLine1 = this.AddressLine1.TrimmedOrNull(),
                AddressLine2 = this.AddressLine2.TrimmedOrNull(),
                City = this.City.TrimmedOrNull(),
                State = this.State == null ? null : this.State.ToUpper().TrimmedOrNull(),
                PostalCode = this.PostalCode.TrimmedOrNull(),

                FiscalContactSamAccountName = this.FiscalContactSamAccountName.TrimmedOrNull(),
                ITConactSamAccountName = this.ITConactSamAccountName.TrimmedOrNull(),

                Parent = this.ParentOrganizationId.HasValue ?
                    context.Organizations.Find(this.ParentOrganizationId) : null
            };
        }
    }
}