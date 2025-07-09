using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;    // for IdentityConstants

namespace Timesheets_APP.ViewModels
{
    public class AdminLoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

      
       
    }
}
