using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FinalProject.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int UserId { get; set; }
        public string PaymentMode { get; set; }

        [DisplayName("Order Date")]
        public DateTime CreatedDate { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public Address Address { get; set; }
        public Product Product { get; set; }
    }
}