using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Infrastructure.Extensions;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using MvcValidationExtensions.Attribute;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Models.Clients
{
    public class ClientEditorForm
    {
        public ClientEditorForm()
        {
            Identifiers = new List<Identifier>();
            IdentifierTypes = new List<IdentifierType>();
            Attorneys = new List<Payee>();
            Guardians = new List<Payee>();
        }
        public int? ClientId { get; set; }

        public int? ImageId { get; set; }

        public HttpPostedFileBase Image { get; set; }
        public int ImageX { get; set; }
        public int ImageY { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }

        [Display(Name = "First Name"), Required,
         RegularExpression(InputRegEx.Name, ErrorMessage = "Invalid First Name"),
         MaxLength(255), MinLength(1)]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name"),
         RegularExpression(InputRegEx.Name, ErrorMessage = "Invalid Middle Name"),
         MaxLength(255), MinLength(1)]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name"), Required,
         RegularExpression(InputRegEx.Name, ErrorMessage = "Invalid Last Name"),
         MaxLength(255), MinLength(1)]
        public string LastName { get; set; }

        [RequiredIf("LivingUnitName", AllowEmptyStrings = true, ErrorMessage = "Invalid Living Unit")]
        public int? LivingUnitId { get; set; }

        public string LivingUnitName { get; set; }

        public bool HasClientProperty { get; set; }

        [Display(Name = "Bank Account"),
         RegularExpression(InputRegEx.BankAccount, ErrorMessage = "Invalid Bank Account"),
         MaxLength(255)]
        public string BankAccount { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }

        public IList<Identifier> Identifiers { get; set; }

        public IList<IdentifierType> IdentifierTypes { get; set; }

        public IList<Payee> Attorneys { get; set; }
        public IList<Payee> Guardians { get; set; }

        public class Identifier
        {
            [Display(Name = "System"),
             Required(AllowEmptyStrings = false, ErrorMessage = "System is Required")]
            public int TypeId { get; set; }
            public int? Id { get; set; }

            [Display(Name = "System Id"),
             Required(AllowEmptyStrings = false, ErrorMessage = "System Id is Required")]
            public string Value { get; set; }
        }

        public class IdentifierType
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class Payee
        {
            public Payee()
            {
                Add = true;
            }
            public int Id { get; set; }
            public string Name { get; set; }
            public bool Add { get; set; }
        }

        public Client BuildClient(ApplicationDbContext context)
        {
            return new Client
            {
                ImageId = this.ImageId,
                FirstName = this.FirstName.TrimmedOrNull(),
                MiddleName = this.MiddleName.TrimmedOrNull(),
                LastName = this.LastName.TrimmedOrNull(),
                HasClientProperty = this.HasClientProperty,
                Comments = this.Comments.TrimmedOrNull(),
                Identifiers = GetClientIdentifiers(context)
            };
        }

        private ICollection<ClientIdentifier> GetClientIdentifiers(ApplicationDbContext context)
        {
            var clientIdentifierTypes = GetClientIdentifierTypes(context);

            return this.Identifiers
                .Select(i => new ClientIdentifier
                {
                    Id = i.Id.GetValueOrDefault(),
                    Value = i.Value,
                    ClientIdentifierType = clientIdentifierTypes.First(t => t.Id == i.TypeId)

                })
                .ToList();
        }

        private ICollection<ClientIdentifierType> GetClientIdentifierTypes(ApplicationDbContext context)
        {
            var selectedIdentifierTypeIds = this.Identifiers.Select(x => x.TypeId)
                .ToArray();

            return context.ClientIdentifierTypes
                .Where(x => selectedIdentifierTypeIds.Contains(x.Id))
                .ToArray();
        }

        public void UpdateClient(Client client, ApplicationDbContext context)
        {
            client.ImageId = this.ImageId;
            client.FirstName = this.FirstName.TrimmedOrNull();
            client.MiddleName = this.MiddleName.TrimmedOrNull();
            client.LastName = this.LastName.TrimmedOrNull();
            client.HasClientProperty = this.HasClientProperty;
            client.Comments = this.Comments.TrimmedOrNull();

            UpdateIdentifiers(client, context);
        }

        private void UpdateIdentifiers(Client client, ApplicationDbContext context)
        {
            var identifierTypes = GetClientIdentifierTypes(context)
                .ToDictionary(t => t.Id, t => t);

            //remove deleted identifiers
            client.Identifiers.Where(i => this.Identifiers.All(o => o.Id != i.Id))
                .ToArray()
                .ForEach(i =>
                {
                    client.Identifiers.Remove(i);
                    context.ClientIdentifiers.Remove(i);
                });

            //edit existing identifiers
            client.Identifiers.Where(i => this.Identifiers.Any(o => o.Id == i.Id))
                .ToArray()
                .ForEach(i => { i.Value = this.Identifiers.Single(o => o.Id == i.Id).Value; });

            //add new identifiers
            this.Identifiers.Where(i => !i.Id.HasValue)
                .ForEach(i =>
                {
                    client.Identifiers.Add(new ClientIdentifier
                    {
                        ClientIdentifierType = identifierTypes[i.TypeId],
                        Value = i.Value
                    });
                });
        }

        public void UpdateResidency(Residency residency, ApplicationDbContext context)
        {
            UpdateLivingUnit(residency, context);
            UpdateAttorneys(residency, context);
            UpdateGuardians(residency, context);
        }

        private void UpdateLivingUnit(Residency residency, ApplicationDbContext context)
        {

            if (this.LivingUnitId.HasValue)
            {
                var livingUnit = context.LivingUnits.FirstOrDefault(l => l.Id == this.LivingUnitId);
                residency.LivingUnit = livingUnit;
            }
            else
            {
                residency.LivingUnitId = new int?();
                residency.LivingUnit = null;
            }
        }

        private void UpdateAttorneys(Residency residency, ApplicationDbContext context)
        {

            //remove deleted attorneys
            var idsToRemove = this.Attorneys.Where(a => a.Add != true).Select(a => a.Id);
            residency.Attorneys.Where(a => !idsToRemove.Contains(a.Id))
                .ToArray()
                .ForEach(a =>
                {
                    residency.Attorneys.Remove(a);
                });

            //add new attorneys
            var idsToAdd = this.Attorneys.Where(a => a.Add).Select(a => a.Id);
            context.Payees.Where(p => idsToAdd.Contains(p.Id))
                .ForEach(p =>
                {
                    residency.Attorneys.Add(p);
                });
        }

        private void UpdateGuardians(Residency residency, ApplicationDbContext context)
        {
            //remove deleted guardians
            var idsToRemove = this.Guardians.Where(a => a.Add != true).Select(a => a.Id);
            residency.Guardians.Where(a => !idsToRemove.Contains(a.Id))
                .ToArray()
                .ForEach(a =>
                {
                    residency.Guardians.Remove(a);
                });

            //add new attorneys
            var idsToAdd = this.Guardians.Where(a => a.Add).Select(a => a.Id);
            context.Payees.Where(p => idsToAdd.Contains(p.Id))
                .ForEach(p =>
                {
                    residency.Guardians.Add(p);
                });
        }

        public static ClientEditorForm FromClient(Client client, Organization organization, IdentifierType[] identifierTypes)
        {
            var residency = client.Residencies.First(r => r.OrganizationId == organization.Id);
            var livingUnit = residency.LivingUnit;
            return new ClientEditorForm
            {
                ClientId = client.Id,
                ImageId = client.ImageId,
                FirstName = client.FirstName,
                MiddleName = client.MiddleName,
                LastName = client.LastName,
                HasClientProperty = client.HasClientProperty,
                Comments = client.Comments,
                LivingUnitId = livingUnit != null ? livingUnit.Id : new int?(),
                LivingUnitName = livingUnit != null ? livingUnit.Name : string.Empty,
                Attorneys = residency.Attorneys
                    .Select(a => new Payee { Id = a.Id, Name = a.Name, Add = false }).ToArray(),
                Guardians = residency.Guardians
                    .Select(a => new Payee { Id = a.Id, Name = a.Name, Add = false }).ToArray(),
                Identifiers = client.Identifiers
                    .Select(i => new Identifier
                    {
                        Id = i.Id,
                        Value = i.Value,
                        TypeId = i.ClientIdentifierType.Id
                    }).ToArray(),
                IdentifierTypes = identifierTypes
            };
        }
    }
}