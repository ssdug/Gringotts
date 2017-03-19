using Wiz.Gringotts.UIWeb.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wiz.Gringotts.UIWeb.Tests.Models
{
    [TestClass]
    public class SearchPagerTests
    {
        [TestMethod]
        public void To_string_format()
        {
            var item = new SearchPager
            {
                Page = 2,
                PageSize = 5,
                IsActive = false,
                Search = "foo"

            };

            var result = item.ToString();

            Assert.IsTrue(result.Contains(item.Page.ToString()));
            Assert.IsTrue(result.Contains(item.PageSize.ToString()));
            Assert.IsTrue(result.Contains(item.IsActive.ToString()));
            Assert.IsTrue(result.Contains(item.Search));
        }
    }
}
