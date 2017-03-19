using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Tests.Helpers;
using Wiz.Gringotts.UIWeb.Controllers;
using Wiz.Gringotts.UIWeb.Infrastructure.ActiveDirectory;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Infrastructure.Security;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Tests.Controllers
{

    [TestClass]
    public class ValidationControllerTests
    {
        private ILogger logger;
        private IMediator mediator;
        private ValidationController validationController;

        [TestInitialize]
        public void Init()
        {
            mediator = Substitute.For<IMediator>();
            logger = Substitute.For<ILogger>();
            validationController = new ValidationController(mediator)
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod]
        public async Task Can_validate_an_group()
        {

            mediator.SendAsync(Arg.Any<ValidateActiveDirectoryGroupCommand>())
                .Returns(Task.FromResult((ICommandResult)new SuccessResult(true)));

            var result = await validationController.LdapGroup(groupName: "dummytext") as ViewResult;
            dynamic data = result.Model;

            Assert.IsTrue(data);

        }

        [TestMethod]
        public async Task Can_validate_a_group_failure()
        {

            mediator.SendAsync(Arg.Any<ValidateActiveDirectoryGroupCommand>())
                .Returns(Task.FromResult((ICommandResult)new FailureResult(string.Empty)));

            var result = await validationController.LdapGroup(groupName: "dummytext") as ViewResult;
            dynamic data = result.Model;

            Assert.AreEqual(data, string.Empty);

        }

        [TestMethod]
        public async Task Can_validate_an_group_including_application_roles()
        {

            mediator.SendAsync(Arg.Any<ValidateActiveDirectoryGroupCommand>())
                .Returns(Task.FromResult((ICommandResult)new SuccessResult(true)));

            var result = await validationController.LdapGroup(groupName: ApplicationRoles.Developer) as ViewResult;
            dynamic data = result.Model;

            Assert.IsTrue(data);
        }

        [TestMethod]
        public async Task Can_validate_an_group_excluding_application_roles()
        {
            mediator.SendAsync(Arg.Any<ValidateActiveDirectoryGroupCommand>())
                .Returns(Task.FromResult((ICommandResult)new SuccessResult(true)));

            var result = await validationController.LdapGroup(groupName: ApplicationRoles.Developer, allowApplicationGroups: false) as ViewResult;
            dynamic data = result.Model;

            Assert.IsTrue(data);

            mediator.Received()
                .SendAsync(Arg.Is<ValidateActiveDirectoryGroupCommand>(c => c.AllowApplicationGroups == false))
                .IgnoreAwaitForNSubstituteAssertion();
        }

        [TestMethod]
        public async Task Can_validate_a_user()
        {
            validationController.ControllerContext = Substitute.For<ControllerContext>();

            validationController.Request.QueryString.Returns(new NameValueCollection
            {
                {"foosamaccountname", "foo"}
            });

            mediator.SendAsync(Arg.Any<ValidateActiveDirectoryUserCommand>())
                .Returns(Task.FromResult((ICommandResult)new SuccessResult(true)));


            var result = await validationController.LdapUser() as ViewResult;
            dynamic data = result.Model;

            Assert.IsTrue(data);
        }

        [TestMethod]
        public async Task Can_validate_a_user_failure()
        {
            validationController.ControllerContext = Substitute.For<ControllerContext>();

            validationController.Request.QueryString.Returns(new NameValueCollection
            {
                {"foosamaccountname", "foo"}
            });

            mediator.SendAsync(Arg.Any<ValidateActiveDirectoryUserCommand>())
                .Returns(Task.FromResult((ICommandResult)new FailureResult(string.Empty)));


            var result = await validationController.LdapUser() as ViewResult;
            dynamic data = result.Model;

            Assert.AreEqual(data, string.Empty);
        }
    }
}