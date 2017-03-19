using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Tests.Helpers;
using Wiz.Gringotts.UIWeb.Controllers;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Models.Organizations;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Tests.Controllers
{
    [TestClass]
    public class OrganizationsControllerTests
    {
        private OrganizationsController controller;
        private IMediator mediator;

        [TestInitialize]
        public void Init()
        {
            mediator = Substitute.For<IMediator>();
            controller = new OrganizationsController(mediator)
            {
                Logger = Substitute.For<ILogger>()
            };
        }


        [TestMethod]
        public async Task Get_Index_without_id_should_use_default_id()
        {
            var id = new int?();
            mediator.SendAsync(Arg.Any<OrganizationDetailsQuery>())
                .Returns(Task.FromResult(new OrganizationDetails()));

            var result = await controller.Index(id) as ViewResult;
            Assert.IsInstanceOfType(result.Model, typeof(OrganizationDetails));


            mediator.Received()
                .SendAsync(Arg.Is<OrganizationDetailsQuery>(f => f.OrganizationId == 1))
                .IgnoreAwaitForNSubstituteAssertion();

        }

        [TestMethod]
        public async Task Get_Index_with_id_should_use_id()
        {
            var id = new int?(42);
            mediator.SendAsync(Arg.Any<OrganizationDetailsQuery>())
                .Returns(Task.FromResult(new OrganizationDetails()));

            var result = await controller.Index(id) as ViewResult;
            Assert.IsInstanceOfType(result.Model, typeof(OrganizationDetails));


            mediator.Received()
                .SendAsync(Arg.Is<OrganizationDetailsQuery>(f => f.OrganizationId == id))
                .IgnoreAwaitForNSubstituteAssertion();

        }

        [TestMethod]
        public async Task Get_Select_with_return_url_and_no_tenant_key()
        {
            var tenantKey = string.Empty;
            var returnUrl = "some/url";

            mediator.SendAsync(Arg.Any<SelectTenantOrganizationQuery>())
                .Returns(Task.FromResult(new OrganizationTenantList(returnUrl, Enumerable.Empty<Organization>())));

            var result = await controller.Select(tenantKey, returnUrl) as ViewResult;

            Assert.IsNotInstanceOfType(result, typeof(OrganizationTenantList));

            mediator.Received()
                .SendAsync(Arg.Is<SelectTenantOrganizationQuery>(q => q.ReturnUrl.Equals(returnUrl)))
                .IgnoreAwaitForNSubstituteAssertion();
        }

        [TestMethod]
        public async Task Get_Select_with_return_url_and_tenant_key()
        {
            var tenantKey = "foo";
            var returnUrl = "some/url";

            mediator.SendAsync(Arg.Any<SelectTenantOrganizationCommand>())
                .Returns(Task.FromResult(new SuccessResult("some/other/url") as ICommandResult));

            var result = await controller.Select(tenantKey, returnUrl) as RedirectResult;

            Assert.IsNotNull(result);

            Assert.IsNotNull(result.Url);

            mediator.Received()
                .SendAsync(Arg.Is<SelectTenantOrganizationCommand>(q => q.ReturnUrl.Equals(returnUrl) && q.TenantKey.Equals(tenantKey)))
                .IgnoreAwaitForNSubstituteAssertion();
        }

        [TestMethod]
        public async Task Get_Select_with_return_url_and_tenant_key_failure()
        {
            var tenantKey = "foo";
            var returnUrl = "some/url";
            var failureMessage = "i failed";

            mediator.SendAsync(Arg.Any<SelectTenantOrganizationCommand>())
                .Returns(Task.FromResult(new FailureResult(failureMessage) as ICommandResult));

            var result = await controller.Select(tenantKey, returnUrl) as ViewResult;

            Assert.IsNotNull(result);

            Assert.IsTrue(result.ViewData.ModelState.Values.Any(d => d.Errors.Any(e => e.ErrorMessage.Equals(failureMessage))));

            mediator.Received()
                .SendAsync(Arg.Is<SelectTenantOrganizationCommand>(q => q.ReturnUrl.Equals(returnUrl) && q.TenantKey.Equals(tenantKey)))
                .IgnoreAwaitForNSubstituteAssertion();
        }


        [TestMethod]
        public async Task Get_Index_with_invalid_id_should_404()
        {
            var id = new int?(42);
            mediator.SendAsync(Arg.Any<OrganizationDetailsQuery>())
                .Returns(Task.FromResult(null as OrganizationDetails));

            var result = await controller.Index(id) as HttpNotFoundResult;
            
            Assert.IsNotNull(result);


            mediator.Received()
                .SendAsync(Arg.Is<OrganizationDetailsQuery>(f => f.OrganizationId == id))
                .IgnoreAwaitForNSubstituteAssertion();

        }

        [TestMethod]
        public async Task Get_Create_without_id_should_not_set_parent_id()
        {
            var id = new int?();
            mediator.SendAsync(Arg.Any<OrganizationEditorFormQuery>())
                .Returns(Task.FromResult(new OrganizationEditorForm()));

            var result = await controller.Create(id) as ViewResult;
            Assert.IsInstanceOfType(result.Model, typeof(OrganizationEditorForm));


            mediator.Received()
                .SendAsync(Arg.Is<OrganizationEditorFormQuery>(f => !f.ParentOrganizationId.HasValue))
                .IgnoreAwaitForNSubstituteAssertion();
            
        }

        [TestMethod]
        public async Task Get_Create_with_an_id_should_set_parent_id()
        {
            var id = new int?(1);
            mediator.SendAsync(Arg.Any<OrganizationEditorFormQuery>())
                .Returns(Task.FromResult(new OrganizationEditorForm()));

            var result = await controller.Create(id) as ViewResult;
            Assert.IsInstanceOfType(result.Model, typeof(OrganizationEditorForm));


            mediator.Received()
                .SendAsync(Arg.Is<OrganizationEditorFormQuery>(f => f.ParentOrganizationId == id))
                .IgnoreAwaitForNSubstituteAssertion();

        }

        [TestMethod]
        public async Task Post_Create()
        {
            var form = new OrganizationEditorForm();
            mediator.SendAsync(Arg.Any<AddOrEditOrganizationCommand>())
                .Returns(Task.FromResult(new SuccessResult(42) as ICommandResult));

            var result = await controller.Create(form) as RedirectToRouteResult;

            Assert.IsNotNull(result);

            Assert.AreEqual(result.RouteValues["Id"], 42);

            mediator.Received()
                .SendAsync(Arg.Is<AddOrEditOrganizationCommand>(f => f.Editor == form))
                .IgnoreAwaitForNSubstituteAssertion();
        }

        [TestMethod]
        public async Task Post_Create_with_failure()
        {
            const string message = "ouch";
            var form = new OrganizationEditorForm();
            mediator.SendAsync(Arg.Any<AddOrEditOrganizationCommand>())
                .Returns(Task.FromResult(new FailureResult(message) as ICommandResult));

            var result = await controller.Create(form) as ViewResult;

            Assert.IsNotNull(result);

            Assert.AreEqual(result.Model, form);

            Assert.IsTrue(result.ViewData.ModelState.ContainsKey(string.Empty));

            Assert.IsTrue(result.ViewData.ModelState[string.Empty].Errors
                .Any(e => e.ErrorMessage == message));

            mediator.Received()
                .SendAsync(Arg.Is<AddOrEditOrganizationCommand>(f => f.Editor == form))
                .IgnoreAwaitForNSubstituteAssertion();
        }

        [TestMethod]
        public async Task Post_Create_with_invalid_model()
        {
            var form = new OrganizationEditorForm();
            
            controller.ModelState.AddModelError("any","error");

            var result = await controller.Create(form) as ViewResult;

            Assert.IsNotNull(result);

            Assert.AreEqual(result.Model, form);

            Assert.IsTrue(result.ViewData.ModelState.ContainsKey("any"));

            mediator.DidNotReceive()
                .SendAsync(Arg.Is<AddOrEditOrganizationCommand>(f => f.Editor == form))
                .IgnoreAwaitForNSubstituteAssertion();
        }

        [TestMethod]
        public async Task Get_Edit_without_id_should_fail()
        {
            var id = new int?();
           

            var result = await controller.Edit(id) as HttpStatusCodeResult;
            
            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, (int)HttpStatusCode.BadRequest);


            mediator.DidNotReceive()
                .SendAsync(Arg.Any<OrganizationEditorFormQuery>())
                .IgnoreAwaitForNSubstituteAssertion();

        }

        [TestMethod]
        public async Task Get_Edit_with_an_id_should_set_organization_id()
        {
            var id = new int?(1);
            mediator.SendAsync(Arg.Any<OrganizationEditorFormQuery>())
                .Returns(Task.FromResult(new OrganizationEditorForm()));

            var result = await controller.Edit(id) as ViewResult;
            Assert.IsInstanceOfType(result.Model, typeof(OrganizationEditorForm));


            mediator.Received()
                .SendAsync(Arg.Is<OrganizationEditorFormQuery>(f => f.OrganizationId == id))
                .IgnoreAwaitForNSubstituteAssertion();

        }

        [TestMethod]
        public async Task Post_Edit()
        {
            var form = new OrganizationEditorForm();
            mediator.SendAsync(Arg.Any<AddOrEditOrganizationCommand>())
                .Returns(Task.FromResult(new SuccessResult(42) as ICommandResult));

            var result = await controller.Edit(form) as RedirectToRouteResult;

            Assert.IsNotNull(result);

            Assert.AreEqual(result.RouteValues["Id"], 42);

            mediator.Received()
                .SendAsync(Arg.Is<AddOrEditOrganizationCommand>(f => f.Editor == form))
                .IgnoreAwaitForNSubstituteAssertion();
        }

        [TestMethod]
        public async Task Post_Edit_with_failure()
        {
            const string message = "ouch";
            var form = new OrganizationEditorForm();
            mediator.SendAsync(Arg.Any<AddOrEditOrganizationCommand>())
                .Returns(Task.FromResult(new FailureResult(message) as ICommandResult));

            var result = await controller.Edit(form) as ViewResult;

            Assert.IsNotNull(result);

            Assert.AreEqual(result.Model, form);

            Assert.IsTrue(result.ViewData.ModelState.ContainsKey(string.Empty));

            Assert.IsTrue(result.ViewData.ModelState[string.Empty].Errors
                .Any(e => e.ErrorMessage == message));

            mediator.Received()
                .SendAsync(Arg.Is<AddOrEditOrganizationCommand>(f => f.Editor == form))
                .IgnoreAwaitForNSubstituteAssertion();
        }

        [TestMethod]
        public async Task Post_Edit_with_invalid_model()
        {
            var form = new OrganizationEditorForm();

            controller.ModelState.AddModelError("any", "error");

            var result = await controller.Edit(form) as ViewResult;

            Assert.IsNotNull(result);

            Assert.AreEqual(result.Model, form);

            Assert.IsTrue(result.ViewData.ModelState.ContainsKey("any"));

            mediator.DidNotReceive()
                .SendAsync(Arg.Is<AddOrEditOrganizationCommand>(f => f.Editor == form))
                .IgnoreAwaitForNSubstituteAssertion();
        }

    }
}