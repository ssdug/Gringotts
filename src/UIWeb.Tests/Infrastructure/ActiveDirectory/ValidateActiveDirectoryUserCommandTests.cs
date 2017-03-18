using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.ActiveDirectory;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Infrastructure.ActiveDirectory
{
    [TestClass]
    public class ValidateActiveDirectoryUserCommandTests
    {
        private ILdapProvider ldapProvider;
        private IAsyncRequestHandler<ValidateActiveDirectoryUserCommand, ICommandResult> handler;


        [TestInitialize]
        public void Init()
        {
            ldapProvider = Substitute.For<ILdapProvider>();
            handler = new ValidateActiveDirectoryUserCommandHandler(ldapProvider)
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.ActiveDirectory)]
        public async Task Can_check_if_a_user_exists()
        {
            var command = new ValidateActiveDirectoryUserCommand("foo");

            ldapProvider.UserExists(Arg.Any<string>())
                .Returns(true);

            var result = await handler.Handle(command);

            Assert.IsInstanceOfType(result, typeof(SuccessResult));
            Assert.IsTrue(result.IsSuccess);
            Assert.IsFalse(result.IsFailure);
            Assert.AreEqual(result.Result, true);
        }

        [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.ActiveDirectory)]
        public async Task Can_check_if_a_user_does_not_exists()
        {
            var command = new ValidateActiveDirectoryUserCommand("foo");

            ldapProvider.GroupExists(Arg.Any<string>())
                .Returns(false);

            var result = await handler.Handle(command);

            Assert.IsInstanceOfType(result, typeof(FailureResult));
            Assert.IsFalse(result.IsSuccess);
            Assert.IsTrue(result.IsFailure);
            Assert.AreNotEqual(result.Result, string.Empty);
        }
         
    }
}