using Wiz.Gringotts.UIWeb.Models.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wiz.Gringotts.UIWeb.Tests.Models.Users
{
    [TestClass]
    public class UserModelTests
    {
        [TestMethod]
        public void Display_Name()
        {
            var user = new User
            {
                FirstName = "foo",
                LastName = "baz"
            };

            Assert.IsTrue(user.DisplayName().Contains(user.FirstName));
            Assert.IsTrue(user.DisplayName().Contains(user.LastName));
        }
    }
}