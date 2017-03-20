using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Infrastructure.ActiveDirectory;
using Wiz.Gringotts.UIWeb.Infrastructure.Commands;
using Wiz.Gringotts.UIWeb.Infrastructure.Security;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Tests.Infrastructure.ActiveDirectory
{
    //[TestClass]
    //public class ValidateActiveDirectoryGroupCommandTests
    //{
    //    private ILdapProvider ldapProvider;
    //    private IAsyncRequestHandler<ValidateActiveDirectoryGroupCommand, ICommandResult> handler;

    //    [TestInitialize]
    //    public void Init()
    //    {
    //        ldapProvider = Substitute.For<ILdapProvider>();
    //        handler = new ValidateActiveDirectoryGroupCommandHandler(ldapProvider)
    //        {
    //            Logger = Substitute.For<ILogger>()
    //        };
    //    }

    //    [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.ActiveDirectory)]
    //    public async Task Can_check_if_a_group_exists()
    //    {
    //        var command = new ValidateActiveDirectoryGroupCommand("foo");

    //        ldapProvider.GroupExists(Arg.Any<string>())
    //            .Returns(true);

    //        var result = await handler.Handle(command);

    //        Assert.IsInstanceOfType(result, typeof(SuccessResult));
    //        Assert.IsTrue(result.IsSuccess);
    //        Assert.IsFalse(result.IsFailure);
    //        Assert.AreEqual(result.Result, true);
    //    }

    //    [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.ActiveDirectory)]
    //    public async Task Can_check_if_a_group_does_not_exists()
    //    {
    //        var command = new ValidateActiveDirectoryGroupCommand("foo");

    //        ldapProvider.GroupExists(Arg.Any<string>())
    //            .Returns(false);

    //        var result = await handler.Handle(command);

    //        Assert.IsInstanceOfType(result, typeof(FailureResult));
    //        Assert.IsFalse(result.IsSuccess);
    //        Assert.IsTrue(result.IsFailure);
    //        Assert.AreNotEqual(result.Result, string.Empty);
    //    }

    //    [TestMethod, TestCategory(Categories.Integration), TestCategory(Categories.ActiveDirectory)]
    //    public async Task Can_restrict_application_role_groups()
    //    {
    //        var command = new ValidateActiveDirectoryGroupCommand(ApplicationRoles.Developer, allowApplicationGroups:false);

    //        var result = await handler.Handle(command);

    //        Assert.IsInstanceOfType(result, typeof(FailureResult));
    //        Assert.IsFalse(result.IsSuccess);
    //        Assert.IsTrue(result.IsFailure);
    //        Assert.AreNotEqual(result.Result, string.Empty);

    //        ldapProvider.DidNotReceive()
    //            .GroupExists(Arg.Any<string>());
    //    }

    //}
}