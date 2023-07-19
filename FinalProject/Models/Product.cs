using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FinalProject.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [DisplayName("Product Title")]
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        public int Price { get; set; }

        public int CategoryId { get; set; }
        
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public int BrandId { get; set; }

        [ForeignKey("BrandId")]
        public Brand Brand { get; set; }

        public Inventory Inventory { get; set; }

        public List<Cart> Carts { get; set; }

        public List<Order>  Orders { get; set; }

        public List<ProductImage> ProductImages { get; set; }

    }
}