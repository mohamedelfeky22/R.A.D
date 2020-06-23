using R.A.D.Models.Auction;
using R.A.D.Models.Rent;
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
    public class RentVM
    {
        public RentVM()
        {
        }
        public RentVM(Rent_Product product)
        {
            Id = product.Id;
            Name = product.Name;
            Description = product.Description;
            Price = product.Price;
            Rent_Started = DateTime.Now;
            Renting_Period = product.Renting_Period;
            ImageName = product.ImageName;
            CategoryId = product.CategoryId;
            UserId = product.UserId;
            AdminId = product.AdminId;

    
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int CategoryId { get; set; }

        public string UserId { get; set; }
        public string AdminId { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
        public DateTime? Rent_Started { get; set; }

        public string Renting_Period { get; set; }

        public string ImageName { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<string> GalleryImages { get; set; }

    }
}
