using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb;
using Wiz.Gringotts.UIWeb.Filters;
using Wiz.Gringotts.UIWeb.Infrastructure.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Wiz.Gringotts.UIWeb.Tests.Filters
{
    [TestClass]
    public class AuthorizeTests
    {

        [TestMethod]
        public void Valid_Claim_Should_Not_Fail()
        {
            var authorizationContext = GetAuthorizationContext();
            var authorizer = new Authorize(typeof (string), Permissions.Action.Read)
            {
                Permissions = GetPermissions(),
                PrincipalProvider = GetPrincipalProvider(),
                Logger = Substitute.For<ILogger>()
            };

            authorizer.OnAuthorization(authorizationContext);

            Assert.IsNull(authorizationContext.Result);
        }

        [TestMethod]
        public void Invalid_Claim_Should_Fail()
        {
            var authorizationContext = GetAuthorizationContext();
            var authorizer = new Authorize(typeof(string), Permissions.Action.Create)
            {
                Permissions = GetPermissions(),
                PrincipalProvider = GetPrincipalProvider(),
                Logger = Substitute.For<ILogger>()
            };

            authorizer.OnAuthorization(authorizationContext);

            Assert.IsNotNull(authorizationContext.Result);
            Assert.IsInstanceOfType(authorizationContext.Result, typeof(HttpForbiddenResult));
        }

        [TestMethod]
        public void Should_Reject_Anonymous_User()
        {
            var authorizationContext = GetAuthorizationContext(anonymous: true);
            var authorizer = new Authorize(typeof(string), Permissions.Action.Read)
            {
                Permissions = GetPermissions(),
                PrincipalProvider = GetPrincipalProvider(anonymous: true),
                Logger = Substitute.For<ILogger>()
            };

            authorizer.OnAuthorization(authorizationContext);

            Assert.IsNotNull(authorizationContext.Result);
            Assert.IsInstanceOfType(authorizationContext.Result, typeof(HttpForbiddenResult));
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void Missing_Permissions_Should_Throw()
        {
            var authorizationContext = GetAuthorizationContext();
            var authorizer = new Authorize(typeof(string))
            {
                Permissions = null,
                PrincipalProvider = GetPrincipalProvider(),
                Logger = Substitute.For<ILogger>()
            };
            
            authorizer.OnAuthorization(authorizationContext);

            Assert.Fail("Authorizer should throw an exception");
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void Missing_Principal_Provider_Should_Throw()
        {
            var authorizationContext = GetAuthorizationContext();
            var authorizer = new Authorize(typeof(string))
            {
                Permissions = GetPermissions(),
                PrincipalProvider = null,
                Logger = Substitute.For<ILogger>()
            };

            authorizer.OnAuthorization(authorizationContext);

            Assert.Fail("Authorizer should throw an exception");
        }

        private Permissions GetPermissions()
        {
            return new Permissions(new TestPermissionSet())
            {
                Logger = Substitute.For<ILogger>()
            };
        }

        private IPrincipalProvider GetPrincipalProvider(bool anonymous = false)
        {
            var provider = Substitute.For<IPrincipalProvider>();
            provider.GetCurrent().Returns(anonymous ? null : GetPrincipal());

            return provider;
        }

        private IPrincipal GetPrincipal()
        {
            return new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] {
                        new Claim(ClaimTypes.Role, ApplicationRoles.Developer)},
                    "Basic"));
        }

        private AuthorizationContext GetAuthorizationContext(bool anonymous = false)
        {
            var context = Substitute.For<AuthorizationContext>();
            
            context.HttpContext.User.Returns(anonymous ? null : GetPrincipal());

            return context;
        }

        private class TestPermissionSet : IPermissionSetProvider
        {
            public IDictionary<Type, IDictionary<Permissions.Action, IEnumerable<string>>> GetPermissionSets()
            {
                return new Dictionary<Type, IDictionary<Permissions.Action, IEnumerable<string>>>
                {
                    {typeof (string), new Dictionary<Permissions.Action, IEnumerable<string>>
                    {
                        {Permissions.Action.Read, new[] {ApplicationRoles.ReadOnly, ApplicationRoles.Developer}}
                    }
                    }
                };
            }
        }
    }
}