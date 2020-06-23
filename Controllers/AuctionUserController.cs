using PagedList;
using R.A.D.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using R.A.D.Models.Donate;
using System.IO;
using R.A.D.Models.ViewModels;
using R.A.D.Models.Auction;
using System;
using System.Web.Services.Description;
using System.Web.WebPages;
using Microsoft.AspNet.Identity;

namespace R.A.D.Controllers
{
    [Authorize(Roles = "User")]
    public class AuctionUserController : Controller
    {

        // GET: AuctionUser
        public ActionResult AuctionProductsList(int? page, int? Id = 0)
        {
            //declare a list of products of products vm
            List<AuctionVM> ListOfProductVM;

            //set page number 

            var pageNumber = page ?? 1;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //init the list 
                ListOfProductVM = db.Auction_Product.ToArray()                   
                    .Select(x => new AuctionVM(x))
                    .ToList();            

            }

            
            ViewBag.OnePageOfProducts = ListOfProductVM.ToPagedList(pageNumber, 20);

           
            return View(ListOfProductVM);
        }





        public ActionResult AuctionDetailsProduct(int Id)
        {
            
            AuctionVM model;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
               
                Auction_Product auction_Product = db.Auction_Product.Find(Id);
               
                if (auction_Product.LastPrice == null)
                {
                    auction_Product.LastPrice =auction_Product.StartingPrice;
                }
                else
                {
                    auction_Product.LastPrice = auction_Product.LastPrice;
                }


                //make sure product exists
                if (auction_Product == null)
                {
                    return Content("That product does not exist");
                }

                //init model

                model = new AuctionVM(auction_Product);

               
                //get all gallery images

                model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + Id ))
                        .Select(fn => Path.GetFileName(fn));
            }
            return View(model);
        }

      
        [HttpPost]
        public ActionResult RaisePrice(int id , int RaisePrice )
        
        {          
           
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
               
                var product = db.Auction_Product.Find(id);
                product.UserId = User.Identity.GetUserId();

                if (product.LastPrice == null)
                {
                    product.LastPrice = product.StartingPrice;
                }

                if (RaisePrice > product.LastPrice)
                {
                    product.LastPrice = RaisePrice;

                    db.SaveChanges();
                    TempData["SM"] = "Good Luck You Raised The Price";
                    return RedirectToAction(nameof(AuctionDetailsProduct) , new { Id=id } );
                }
                else
                {
                    TempData["SM"] = "Failed Please enter a Price higher than the Last Price";
                    return RedirectToAction(nameof(AuctionDetailsProduct), new { Id = id });
                }
               
            }
        }
    }
}