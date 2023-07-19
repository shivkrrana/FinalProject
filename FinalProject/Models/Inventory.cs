using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FinalProject.Models
{
    public class Inventory
    {
        [Key,ForeignKey("Product")]
        public int InventoryId { get; set; }

        [Required(ErrorMessage ="required")]
        public int Stock { get; set; }

        public Product Product { get; set; }  

    }
}