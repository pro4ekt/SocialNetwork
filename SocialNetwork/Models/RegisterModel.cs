using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SocialNetwork.Models
{
    public class RegisterModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required] 
        [DataType(DataType.Text)]
        public string Address { get; set; }

        [Required]
        [Display(Name = "User Name")]
        [RegularExpression(@"^[A-Za-zА-Яа-я][A-Za-zА-Яа-я0-9]{1,9}$", ErrorMessage = "User Name should be 2-10 letters and should not start with number and be 1 word")]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Info { get; set; }

        [Required]
        public int Age { get; set; }
    }
}