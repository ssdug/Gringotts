namespace Wiz.Gringotts.UIWeb.Models
{
    public static class InputRegEx
    {
        public const string PhoneNumber = @"1?\s*\W?\s*([2-9][0-8][0-9])\s*\W?\s*([2-9][0-9]{2})\s*\W?\s*([0-9]{4})(\se?x?t?(\d*))?";
        public const string Currency = @"(?=.)^\$?(([1-9][0-9]{0,2}(,[0-9]{3})*)|[0-9]+)?(\.[0-9]{1,2})?$";
        public const string PostalCode = @"^\d{5}(-\d{4})?$";
        public const string Name = @"^[a-zA-Z\s-]+$";
        public const string BankAccount = @"^[a-zA-Z0-9\s-]+$";
        public const string Abbreviation = @"^[a-zA-Z]+$";
        public const string OrderNumber = @"^[a-zA-Z0-9\s-]+$";
        public const string Interger = @"^\d+$";
    }
}