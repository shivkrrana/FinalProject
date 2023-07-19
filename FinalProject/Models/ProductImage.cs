using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FinalProject.Models
{
    public class ProductImage
    {
        [Key]
        public int ImageId { get; set; }
        public string FileName { get; set; }
        public byte[] Data { get; set; }

        [NotMapped]
        public HttpPostedFileBase ImageFile { get; set; }
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}