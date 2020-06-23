using R.A.D.Models.Donate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RAD.Models.ViewModels
{
    public class DonationVM
    {
        public DonationVM()
        {
        }
        public DonationVM(Donate_Product product)
        {
            Id = product.Id;
            Name = product.Name;
            Description = product.Description;
            Date = DateTime.Now;
            ImageName = product.ImageName;
            AdminId = product.AdminId;
            UserId = product.UserId;
            CategoryId = product.CategoryId;

        }
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        public DateTime Date { get; set; }

        public string ImageName { get; set; }


        public string AdminId { get; set; }
        public string UserId { get; set; }



        [DisplayName("Category Name")]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<string> GalleryImages { get; set; }
    }
}