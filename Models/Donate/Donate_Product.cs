using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace R.A.D.Models.Donate
{
    [Table("DonateProductsTbl")]
    public class Donate_Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime? Date { get; set; }
        public string Description { get; set; }

        public int CategoryId { get; set; }

        public string ImageName { get; set; }

       
        public string AdminId { get; set; }
        public string UserId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Donate_Category Donate_Category { get; set; }
    }
}