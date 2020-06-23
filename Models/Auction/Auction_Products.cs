using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace R.A.D.Models.Auction
{
    [Table("AuctionProductsTbl")]
    public class Auction_Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string CountryOfOrgin { get; set; }
        public string Details { get; set; }
        public string Description { get; set; }

        [DataType(DataType.Currency)]
        public int StartingPrice { get; set; }

        [DataType(DataType.Currency)]
        public int? LastPrice { get; set; }

        public string AdminId { get; set; }
        public string UserId { get; set; }

        public DateTime? Auction_Started { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
        public DateTime? Auction_Ended { get; set; }

        public string ImageName { get; set; }
    }
}