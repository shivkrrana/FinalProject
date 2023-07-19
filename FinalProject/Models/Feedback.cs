using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinalProject.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }

        [Required]
        public string Message { get; set;}
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}