using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinalProject.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public int OrderId { get; set; }
        public string TransactionStatus { get; set; }
        public decimal TransactionAmount { get; set; }
        public string PaymentMode { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}