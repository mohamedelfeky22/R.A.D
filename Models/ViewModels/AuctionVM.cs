using R.A.D.Models.Auction;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace R.A.D.Models.ViewModels
{
    public class AuctionVM
    {
        public AuctionVM()
        {
        }
        public AuctionVM(Auction_Product product)
        {
            Id = product.Id;
            Name = product.Name;
            Description = product.Description;
            CountryOfOrgin = product.CountryOfOrgin;
            Details = product.Details;
            StartingPrice = product.StartingPrice;
            LastPrice = product.LastPrice;
            UserId = product.UserId;
            AdminId = product.AdminId;
            Auction_Started = DateTime.Now;
            Auction_Ended = product.Auction_Ended;
            ImageName = product.ImageName;
            UserId = product.UserId;
            AdminId = product.AdminId;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string CountryOfOrgin { get; set; }
        public string Details { get; set; }
        public string Description { get; set; }
        public int StartingPrice { get; set; }
        public int? LastPrice { get; set; }
        public string UserId { get; set; }
        public string AdminId { get; set; }
        public DateTime? Auction_Started { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
        public DateTime? Auction_Ended { get; set; }

        public string ImageName { get; set; }

        public IEnumerable<string> GalleryImages { get; set; }
    }
}
