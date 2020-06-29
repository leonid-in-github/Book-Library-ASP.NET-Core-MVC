using Book_Library_Repository_EF_Core.Models.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Book_Library_ASP.NET_Core_MVC.Models.Accounts
{
    public class UserModel
    {
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        public static explicit operator UserModel(DisplayUserModel model)
        {
            return new UserModel { Login = model.Login, FirstName = model.FirstName, LastName = model.LastName, Email = model.Email };
        }
    }
}
