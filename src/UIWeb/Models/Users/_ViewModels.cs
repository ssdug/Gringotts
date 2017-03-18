using System.ComponentModel.DataAnnotations;

namespace Wiz.Gringotts.UIWeb.Models.Users
{
    public class User
    {
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Display(Name = "Phone")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        public string DisplayName()
        {
            return string.Format("{0} {1}", FirstName, LastName);
        } 
    }
}