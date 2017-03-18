using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Helpers;
using Wiz.Gringotts.UIWeb.Models;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Models.Organizations
{
    [TestClass]
    public class OrganizationEditorFormValidationHandlerTests
    {
        private OrganizationEditorFormValidatorHandler handler;
        private ISearch<Organization> organizations;

        [TestInitialize]
        public void Init()
        {
            organizations = Substitute.For<ISearch<Organization>>();
            handler = new OrganizationEditorFormValidatorHandler(organizations)
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod]
        public async Task Can_handle_request()
        {
            var command = GetCommand(add: true);

            handler.Validators = Enumerable.Empty<Action<AddOrEditOrganizationCommand>>();

            await handler.Handle(command);

            Assert.IsTrue(command.ModelState.IsValid);
        }

        [TestMethod]
        public async Task Can_handle_invalid_request()
        {
            var command = GetCommand(add: true);

            command.ModelState.AddModelError("foo","bar");

            handler.Validators = Enumerable.Empty<Action<AddOrEditOrganizationCommand>>();

            await handler.Handle(command);

            Assert.IsFalse(command.ModelState.IsValid);
        }

        [TestMethod]
        public void Duplicate_name_fails()
        {
            var name = "foo";
            var command = GetCommand(add: true);

            command.Editor.Name = name;

            organizations.All()
                .Returns(new[] { new Organization { Name = name } }.AsAsyncQueryable());

            handler.EnsureNameIsUnique(command);

            Assert.IsFalse(command.ModelState.IsValid);

            Assert.IsTrue(command.ModelState.ContainsKey("Name"));
        }

        [TestMethod]
        public void Unique_name_passes()
        {
            var name = "foo";
            var command = GetCommand(add: true);

            command.Editor.Name = name;

            organizations.All()
                .Returns(Enumerable.Empty<Organization>().AsAsyncQueryable());

            handler.EnsureNameIsUnique(command);

            Assert.IsTrue(command.ModelState.IsValid);
        }

        [TestMethod]
        public void Duplicate_group_fails()
        {
            var group = "foo";
            var command = GetCommand(add: true);

            command.Editor.GroupName = group;

            organizations.All()
                .Returns(new[] {new Organization {Id = 42, Name = "bar", GroupName = group}}.AsAsyncQueryable());

            handler.EnsureGroupIsUnique(command);

            Assert.IsFalse(command.ModelState.IsValid);

            Assert.IsTrue(command.ModelState.ContainsKey("Group"));
        }

        [TestMethod]
        public void Unique_group_passes()
        {
            var group = "foo";
            var command = GetCommand(add: true);

            command.Editor.GroupName = group;
            organizations.All()
                .Returns(Enumerable.Empty<Organization>().AsAsyncQueryable());

            handler.EnsureGroupIsUnique(command);

            Assert.IsTrue(command.ModelState.IsValid);
        }

        [TestMethod]
        public void Duplicate_abbreviation_fails()
        {
            var abbreviation = "foo";
            var command = GetCommand(add: true);

            command.Editor.Abbreviation = abbreviation;
            organizations.All()
                .Returns(new[] {new Organization {Id = 42, Abbreviation = abbreviation, Name = "bar", GroupName = "baz"}}
                        .AsAsyncQueryable());

            handler.EnsureAbbreviationIsUnique(command);

            Assert.IsFalse(command.ModelState.IsValid);

            Assert.IsTrue(command.ModelState.ContainsKey("Abbreviation"));
        }

        [TestMethod]
        public void Unique_abbreviation_passes()
        {
            var abbreviation = "foo";
            var command = GetCommand(add: true);

            command.Editor.Abbreviation = abbreviation;
            organizations.All()
                .Returns(Enumerable.Empty<Organization>().AsAsyncQueryable());

            handler.EnsureAbbreviationIsUnique(command);

            Assert.IsTrue(command.ModelState.IsValid);
        }

        private AddOrEditOrganizationCommand GetCommand(bool add = false)
        {
            var form = GetForm(add: add);

            return new AddOrEditOrganizationCommand(form, new ModelStateDictionary());
        }

        private static OrganizationEditorForm GetForm(bool add = false)
        {
            return new OrganizationEditorForm
            {
                OrganizationId = add ? new int?() : 42,
            };
        }
    }
}