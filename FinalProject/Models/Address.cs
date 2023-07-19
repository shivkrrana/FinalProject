using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FinalProject.Models
{
    public class Address
    {
        public int AddressId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [DisplayName("Full name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Mobile No is required")]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Enter valid Mobile No")]
        [DisplayName("Mobile number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Pincode is required")]
        [RegularExpression("^[1-9]{1}\\d{2}\\s?\\d{3}$" , ErrorMessage = "Enter valid Pincode")]
        [DisplayName("Pincode")]
        public string Pincode { get; set; }

        [Required(ErrorMessage = "Address 1 is required")]
        [DisplayName("Flat, House no., Apartment")]
        public string Region1 { get; set; }

        [DisplayName("Area, Street, Sector, Village")]
        public string Region2 { get; set; }

        [DisplayName("Landmark")]
        public string Landmark { get; set; }

        [Required(ErrorMessage = "City/Town is required")]
        [DisplayName("Town/City")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required")]
        [DisplayName("State")]
        public string State { get; set; }

        public int UserId { get; set; }

        //Navigation Property
        [ForeignKey("UserId")]
        public User User { get; set; }

    }
}