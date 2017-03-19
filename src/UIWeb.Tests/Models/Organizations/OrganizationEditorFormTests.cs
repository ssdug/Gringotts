using Wiz.Gringotts.UIWeb.Data;
using Wiz.Gringotts.UIWeb.Models.Features;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Tests.Models.Organizations
{
    [TestClass]
    public class OrganizationEditorFormTests
    {
        private ApplicationDbContext context;

        [TestInitialize]
        public void Init()
        {
            context = Substitute.For<ApplicationDbContext>();
        }

        [TestMethod]
        public void Can_build_from_Oranization()
        {
            var organization = new Organization
            {
                Id = 42,
                Name = "name",
                GroupName = "groupname",
                Abbreviation = "abbreviation",
                Phone = "phone",
                AddressLine1 = "addressline1",
                AddressLine2 = "addressline2",
                City = "city",
                State = "state",
                PostalCode = "postalcode",
                FiscalContactSamAccountName = "fiscalcontactsamaccountname",
                ITConactSamAccountName = "itcontactsamaccountname",
                Parent = new Organization { Id = 24 }
            };

            var result = OrganizationEditorForm.FromOrganization(organization, new Feature[] {});

            Assert.AreEqual(result.OrganizationId, organization.Id);
            Assert.AreEqual(result.Name, organization.Name);
            Assert.AreEqual(result.GroupName, organization.GroupName);
            Assert.AreEqual(result.Abbreviation, organization.Abbreviation);
            Assert.AreEqual(result.Phone, organization.Phone);
            Assert.AreEqual(result.AddressLine1, organization.AddressLine1);
            Assert.AreEqual(result.AddressLine2, organization.AddressLine2);
            Assert.AreEqual(result.City, organization.City);
            Assert.AreEqual(result.State, organization.State);
            Assert.AreEqual(result.PostalCode, organization.PostalCode);
            Assert.AreEqual(result.FiscalContactSamAccountName, organization.FiscalContactSamAccountName);
            Assert.AreEqual(result.ITConactSamAccountName, organization.ITConactSamAccountName);
            Assert.AreEqual(result.ParentOrganizationId, organization.Parent.Id);

        }

        [TestMethod]
        public void Can_build_from_parent_Oranization()
        {
            var organization = new Organization
            {
                Id = 42,
            };

            var result = OrganizationEditorForm.FromParentOrganization(organization, new Feature[] {});

            Assert.AreEqual(result.ParentOrganizationId, organization.Id);

        }

        [TestMethod]
        public void Can_update_an_organization()
        {
            var form = new OrganizationEditorForm
            {
                OrganizationId = 42,
                Name = "name",
                GroupName = "groupname",
                Abbreviation = "abbreviation",
                Phone = "phone",
                AddressLine1 = "addressline1",
                AddressLine2 = "addressline2",
                City = "city",
                State = "state",
                PostalCode = "postalcode",
                FiscalContactSamAccountName = "fiscalcontactsamaccountname",
                ITConactSamAccountName = "itcontactsamaccountname"
            };

            var organization = new Organization();

            form.UpdateOrganization(organization);

            Assert.AreEqual(organization.Name, form.Name);
            Assert.AreEqual(organization.GroupName, form.GroupName);
            Assert.AreEqual(organization.Abbreviation, form.Abbreviation);
            Assert.AreEqual(organization.Phone, form.Phone);
            Assert.AreEqual(organization.AddressLine1, form.AddressLine1);
            Assert.AreEqual(organization.AddressLine2, form.AddressLine2);
            Assert.AreEqual(organization.City, form.City);
            Assert.AreEqual(organization.State, form.State.ToUpper());
            Assert.AreEqual(organization.PostalCode, form.PostalCode);
            Assert.AreEqual(organization.FiscalContactSamAccountName, form.FiscalContactSamAccountName);
            Assert.AreEqual(organization.ITConactSamAccountName, form.ITConactSamAccountName);
        }
         
    }
}