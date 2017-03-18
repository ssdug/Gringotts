using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Models;
using Wiz.Gringotts.UIWeb.Models.Clients;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using PagedList;
using Wiz.Gringotts.UIWeb.Helpers;

namespace Wiz.Gringotts.UIWeb.Controllers
{
    [TestClass]
    public class ClientsControllerTests
    {
        private IMediator mediator;
        private ClientsController controller;

        [TestInitialize]
        public void Init()
        {
            mediator = Substitute.For<IMediator>();
            controller = new ClientsController(mediator)
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod]
        public async Task Get_Index_with_default_pager()
        {
            var pager = new SearchPager();
            var viewModel = new ClientSearchResult { Items = new PagedList<Client>(Enumerable.Empty<Client>(), 1, 10) };
            mediator.SendAsync(Arg.Any<ClientSearchQuery>())
                .Returns(Task.FromResult(viewModel));

            var result = await controller.Index(pager) as ViewResult;

            Assert.AreEqual(result.Model, viewModel);

            mediator.Received()
                .SendAsync(Arg.Is<ClientSearchQuery>(q => q.Pager == pager))
                .IgnoreAwaitForNSubstituteAssertion();

        }

        [TestMethod]
        public async Task Get_Show_with_valid_client_id()
        {
            var clientId = 42;
            var viewModel = new ClientDetails();
            mediator.SendAsync(Arg.Any<ClientDetailsQuery>())
                .Returns(Task.FromResult(viewModel));

            var result = await controller.Show(clientId) as ViewResult;

            Assert.AreEqual(result.Model, viewModel);

            mediator.Received()
                .SendAsync(Arg.Is<ClientDetailsQuery>(q => q.ClientId == clientId))
                .IgnoreAwaitForNSubstituteAssertion();

        }

        [TestMethod]
        public async Task Get_Show_with_invalid_client_id()
        {
            var clientId = 42;
            mediator.SendAsync(Arg.Any<ClientDetailsQuery>())
                .Returns(Task.FromResult(null as ClientDetails));

            var result = await controller.Show(clientId);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues["Action"],"Index");

            mediator.Received()
                .SendAsync(Arg.Is<ClientDetailsQuery>(q => q.ClientId == clientId))
                .IgnoreAwaitForNSubstituteAssertion();

        }

        [TestMethod]
        public async Task Get_Create()
        {
            var form = new ClientEditorForm();
            mediator.SendAsync(Arg.Any<ClientEditorFormQuery>())
                .Returns(Task.FromResult(form));

            var result = await controller.Create() as ViewResult;

            Assert.AreEqual(result.Model, form);

            mediator.Received()
                .SendAsync(Arg.Any<ClientEditorFormQuery>())
                .IgnoreAwaitForNSubstituteAssertion();

        }

        [TestMethod]
        public async Task Post_Create_with_invalid_model_state()
        {
            var form = new ClientEditorForm();

            controller.ModelState.AddModelError("somekey","some error");

            var result = await controller.Create(form) as ViewResult;

            Assert.AreEqual(result.Model, form, "it should redisplay the form");

            mediator.DidNotReceive()
                .SendAsync(Arg.Any<AddOrEditClientCommand>())
                .IgnoreAwaitForNSubstituteAssertion();

        }

        [TestMethod]
        public async Task Post_Create_with_valid_model_state()
        {
            var form = new ClientEditorForm();
            var clientId = 42;

            mediator.SendAsync(Arg.Any<AddOrEditClientCommand>())
                .Returns(Task.FromResult(new SuccessResult(clientId) as ICommandResult));

            var result = await controller.Create(form) as RedirectToRouteResult;

            Assert.IsNotNull(result);

            Assert.AreEqual(result.RouteValues["action"], "Show", "it should redirect to the show page");
            Assert.AreEqual(result.RouteValues["id"], clientId, "it should redirect to the generated client id");
           

        }

        [TestMethod]
        public async Task Post_Create_with_vaid_model_state_failure()
        {
            var form = new ClientEditorForm();
            var failureMesage = "some error text";

            mediator.SendAsync(Arg.Any<AddOrEditClientCommand>())
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
            Assert.AreEqual(result.StatusCode,(int) HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task Get_Edit_with_valid_id()
        {
            var id = new int?(42);
            var form = new ClientEditorForm();
            mediator.SendAsync(Arg.Any<ClientEditorFormQuery>())
                .Returns(Task.FromResult(form));

            var result = await controller.Edit(id) as ViewResult;

            Assert.AreEqual(result.Model, form);

            mediator.Received()
                .SendAsync(Arg.Is<ClientEditorFormQuery>(q => q.ClientId == id))
                .IgnoreAwaitForNSubstituteAssertion();
        }

        [TestMethod]
        public async Task Post_Edit_with_invalid_modelState()
        {
            var form = new ClientEditorForm();

            controller.ModelState.AddModelError("somekey", "some error");

            var result = await controller.Edit(form) as ViewResult;

            Assert.AreEqual(result.Model, form, "it should redisplay the form");

            mediator.DidNotReceive()
                .SendAsync(Arg.Any<AddOrEditClientCommand>())
                .IgnoreAwaitForNSubstituteAssertion();
        }

        [TestMethod]
        public async Task Post_Edit_with_vaid_model_state()
        {
            var form = new ClientEditorForm();
            var clientId = 42;

            mediator.SendAsync(Arg.Any<AddOrEditClientCommand>())
                .Returns(Task.FromResult(new SuccessResult(clientId) as ICommandResult));

            var result = await controller.Edit(form) as RedirectToRouteResult;

            Assert.IsNotNull(result);

            Assert.AreEqual(result.RouteValues["action"], "Show", "it should redirect to the show page");
            Assert.AreEqual(result.RouteValues["id"], clientId, "it should redirect to the generated client id");


        }

        [TestMethod]
        public async Task Post_Edit_with_vaid_model_state_failure()
        {
            var form = new ClientEditorForm();
            var failureMesage = "some error text";

            mediator.SendAsync(Arg.Any<AddOrEditClientCommand>())
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
            mediator.SendAsync(Arg.Any<ToggleClientIsActiveCommand>())
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
            mediator.SendAsync(Arg.Any<ToggleClientIsActiveCommand>())
                .Returns(Task.FromResult(commandResult as ICommandResult));

            var result = await controller.Toggle(id) as RedirectToRouteResult;

            Assert.IsNotNull(result);

            Assert.AreEqual(result.RouteValues["action"], "Show", "it should redirect to the show page");
            Assert.AreEqual(result.RouteValues["id"], id, "it should redirect to the generated client id");
            Assert.Inconclusive("this seems wrong, if we get a failure result, we need to show that failure");
        }
    }
}
