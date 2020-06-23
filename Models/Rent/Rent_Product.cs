using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace R.A.D.Models.Rent
{
    [Table("RentProductsTbl")]
    public class Rent_Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        [DataType(DataType.Currency)]
        public int Price { get; set; }


        public DateTime? Rent_Started { get; set; }



        public string Renting_Period{ get; set; }
        public int CategoryId { get; set; }

        public string AdminId { get; set; }
        public string UserId { get; set; }

        public string ImageName { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Rent_Category Rent_Category { get; set; }

    }
}