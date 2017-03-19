using Wiz.Gringotts.UIWeb.Models.Payees;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wiz.Gringotts.UIWeb.Tests.Models.Payees
{
    [TestClass]
    public class PayeeTests
    {
        [TestMethod]
        public void Display_Address_with_single_address_line()
        {
            var payee = new Payee
            {
                AddressLine1 = "addressline1",
                City = "city",
                State = "state",
                PostalCode = "postalcode"
            };

            var result = payee.DisplayAddress;

            Assert.IsTrue(result.Contains(payee.AddressLine1));
            Assert.IsTrue(result.Contains(payee.City));
            Assert.IsTrue(result.Contains(payee.State));
            Assert.IsTrue(result.Contains(payee.PostalCode));
        }

        [TestMethod]
        public void Display_Address_with_two_address_line()
        {
            var payee = new Payee
            {
                AddressLine1 = "addressline1",
                AddressLine2 = "addressline2",
                City = "city",
                State = "state",
                PostalCode = "postalcode"
            };

            var result = payee.DisplayAddress;

            Assert.IsTrue(result.Contains(payee.AddressLine1));
            Assert.IsTrue(result.Contains(payee.AddressLine2));
            Assert.IsTrue(result.Contains(payee.City));
            Assert.IsTrue(result.Contains(payee.State));
            Assert.IsTrue(result.Contains(payee.PostalCode));
        }
    }
}
