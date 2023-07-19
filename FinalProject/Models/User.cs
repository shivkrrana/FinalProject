using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FinalProject.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Email is required")]
        [RegularExpression("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$",ErrorMessage ="Not valid Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile No is required")]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Enter valid Mobile No")]
        [DisplayName("Mobile number")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Role { get; set; }

        [NotMapped]
        [DisplayName("Confirm Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "This field is required")]
        [Compare("Password", ErrorMessage = "Passwords do NOT match")]
        public string ConfirmPassword { get; set; }


        public List<Cart> Carts { get; set; }
        public List<Address> Addresses{ get; set; }

        public List<Order> Orders { get; set; }


    }
}