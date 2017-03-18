using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Helpers;
using Wiz.Gringotts.UIWeb.Controllers;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Models.Payees;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using PagedList;

namespace Wiz.Gringotts.UIWeb.Controllers
{
    [TestClass]
    public class PayeesControllerTests
    {
        private IMediator mediator;
        private PayeesController controller;

        [TestInitialize]
        public void Init()
        {
            mediator = Substitute.For<IMediator>();
            controller = new PayeesController(mediator)
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod]
        public async Task Get_Index_with_default_pager()
        {
            var pager = new PayeesSearchPager();
            var viewModel = new PayeeSearchResult { Items = new PagedList<Payee>(Enumerable.Empty<Payee>(), 1, 10 ) };
            mediator.SendAsync(Arg.Any<PayeeSearchQuery>())
                .Returns(Task.FromResult(viewModel));

            var result = await controller.Index(pager) as ViewResult;

            Assert.AreEqual(result.Model, viewModel);

            mediator.Received()
                .SendAsync(Arg.Is<PayeeSearchQuery>(q => q.Pager == pager))
                .IgnoreAwaitForNSubstituteAssertion();

        }

        [TestMethod]
        public async Task Get_Show_with_valid_payee_id()
        {
            var payeeId = 42;
            var viewModel = new PayeeDetails();
            mediator.SendAsync(Arg.Any<PayeeDetailsQuery>())
                .Returns(Task.FromResult(viewModel));

            var result = await controller.Show(payeeId) as ViewResult;

            Assert.AreEqual(result.Model, viewModel);

            mediator.Received()
                .SendAsync(Arg.Is<PayeeDetailsQuery>(q => q.PayeeId == payeeId))
                .IgnoreAwaitForNSubstituteAssertion();
        }

        [TestMethod]
        public async Task Get_Show_with_invalid_payee_id()
        {
            var payeeId = 42;
            mediator.SendAsync(Arg.Any<PayeeDetailsQuery>())
                .Returns(Task.FromResult(null as PayeeDetails));

            var result = await controller.Show(payeeId);

            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));

            Assert.IsTrue(((HttpNotFoundResult)result).StatusDescription.Contains(payeeId.ToString()));


            mediator.Received()
                .SendAsync(Arg.Is<PayeeDetailsQuery>(q => q.PayeeId == payeeId))
                .IgnoreAwaitForNSubstituteAssertion();
        }

        [TestMethod]
        public async Task Get_Create()
        {
            var from = new PayeeEditorForm();
            mediator.SendAsync(Arg.Any<PayeeEditorFormQuery>())
                .Returns(Task.FromResult(from));

            var result = await controller.Create() as ViewResult;

            Assert.AreEqual(result.Model, from);

            mediator.Received()
                .SendAsync(Arg.Any<PayeeEditorFormQuery>())
                .IgnoreAwaitForNSubstituteAssertion();
        }

        [TestMethod]
        public async Task Post_Create_with_invalid_model_state()
        {
            var form = new PayeeEditorForm();

            controller.ModelState.AddModelError("somekey", "some error");

            var result = await controller.Create(form) as ViewResult;

            Assert.AreEqual(result.Model, form, "it should redisplay the form");

            mediator.DidNotReceive()
                .SendAsync(Arg.Any<AddOrEditPayeeCommand>())
                .IgnoreAwaitForNSubstituteAssertion();

        }

        [TestMethod]
        public async Task Post_Create_with_valid_model_state()
        {
            var form = new PayeeEditorForm();
            var payeeId = 42;

            mediator.SendAsync(Arg.Any<AddOrEditPayeeCommand>())
                .Returns(Task.FromResult(new SuccessResult(payeeId) as ICommandResult));

            var result = await controller.Create(form) as RedirectToRouteResult;

            Assert.IsNotNull(result);

            Assert.AreEqual(result.RouteValues["action"], "Show", "it should redirect to the show page");
            Assert.AreEqual(result.RouteValues["id"], payeeId, "it should redirect to the generated client id");


        }

        [TestMethod]
        public async Task Post_Create_with_vaid_model_state_failure()
        {
            var form = new PayeeEditorForm();
            var failureMesage = "some error text";

            mediator.SendAsync(Arg.Any<AddOrEditPayeeCommand>())
                .Returns(Task.FromResult(new FailureResult(failureMesage) as ICommandResult));

            var result = await controller.Create(form) as ViewResult;

            Assert.AreEqual(result.Model, form);

            Assert.IsFalse(controller.ModelState.IsValid);

            Assert.IsTrue(controller.ModelState[""]
                .Errors.Any(e => e.ErrorMessage.Equals(failureMesage)));

        }

        [TestMethod]
        public async Task Get_Edit_with_null_id()
        {
            var id = new int?();

            var result = await controller.Edit(id) as HttpStatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task Get_Edit_with_valid_id()
        {
            var id = new int?(42);
            var form = new PayeeEditorForm();
            mediator.SendAsync(Arg.Any<PayeeEditorFormQuery>())
                .Returns(Task.FromResult(form));

            var result = await controller.Edit(id) as ViewResult;

            Assert.AreEqual(result.Model, form);

            mediator.Received()
                .SendAsync(Arg.Is<PayeeEditorFormQuery>(q => q.PayeeId == id))
                .IgnoreAwaitForNSubstituteAssertion();
        }

        [TestMethod]
        public async Task Post_Edit_with_invalid_modelState()
        {
            var form = new PayeeEditorForm();

            controller.ModelState.AddModelError("somekey", "some error");

            var result = await controller.Edit(form) as ViewResult;

            Assert.AreEqual(result.Model, form, "it should redisplay the form");

            mediator.DidNotReceive()
                .SendAsync(Arg.Any<AddOrEditPayeeCommand>())
                .IgnoreAwaitForNSubstituteAssertion();
        }

        [TestMethod]
        public async Task Post_Edit_with_vaid_model_state()
        {
            var form = new PayeeEditorForm();
            var payeeId = 42;

            mediator.SendAsync(Arg.Any<AddOrEditPayeeCommand>())
                .Returns(Task.FromResult(new SuccessResult(payeeId) as ICommandResult));

            var result = await controller.Edit(form) as RedirectToRouteResult;

            Assert.IsNotNull(result);

            Assert.AreEqual(result.RouteValues["action"], "Show", "it should redirect to the show page");
            Assert.AreEqual(result.RouteValues["id"], payeeId, "it should redirect to the generated client id");
        }

        [TestMethod]
        public async Task Post_Edit_with_vaid_model_state_failure()
        {
            var form = new PayeeEditorForm();
            var failureMesage = "some error text";

            mediator.SendAsync(Arg.Any<AddOrEditPayeeCommand>())
                .Returns(Task.FromResult(new FailureResult(failureMesage) as ICommandResult));

            var result = await controller.Edit(form) as ViewResult;

            Assert.AreEqual(result.Model, form);

            Assert.IsFalse(controller.ModelState.IsValid);

            Assert.IsTrue(controller.ModelState[""]
                .Errors.Any(e => e.ErrorMessage.Equals(failureMesage)));

        }

        [TestMethod]
        public async Task Get_Toggle_with_null_id()
        {
            var id = new int?();

            var result = await controller.Toggle(id) as HttpStatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.StatusCode, (int)HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task Get_Toggle_with_valid_id()
        {
            var id = new int?(42);
            var commandResult = new SuccessResult();
            mediator.SendAsync(Arg.Any<TogglePayeeIsActiveCommand>())
                .Returns(Task.FromResult(commandResult as ICommandResult));

            var result = await controller.Toggle(id) as RedirectToRouteResult;

            Assert.IsNotNull(result);

            Assert.AreEqual(result.RouteValues["action"], "Show", "it should redirect to the show page");
            Assert.AreEqual(result.RouteValues["id"], id, "it should redirect to the generated client id");

        }

        [TestMethod]
        public async Task Get_Toggle_with_valid_id_failure()
        {
            var id = new int?(42);
            var commandResult = new FailureResult(string.Empty);
            mediator.SendAsync(Arg.Any<TogglePayeeIsActiveCommand>())
                .Returns(Task.FromResult(commandResult as ICommandResult));

            var result = await controller.Toggle(id) as RedirectToRouteResult;

            Assert.IsNotNull(result);

            Assert.AreEqual(result.RouteValues["action"], "Show", "it should redirect to the show page");
            Assert.AreEqual(result.RouteValues["id"], id, "it should redirect to the generated client id");
            Assert.Inconclusive("this seems wrong, if we get a failure result, we need to show that failure");
        }
    }
}