using Microsoft.AspNet.Identity;
using PagedList;
using R.A.D.Models;
using R.A.D.Models.Rent;
using R.A.D.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace R.A.D.Controllers
{
    public class RentUserController : Controller
    {

        // GET: AuctionUser
        public ActionResult RentProductsList(int? page, int? catId = 0)
        {
            //declare a list of products of products vm
            List<RentVM> ListOfProductVM;

            //set page number 

            var pageNumber = page ?? 1;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //init the list 
                ListOfProductVM = db.Rent_Product.ToArray()
                    .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                    .Select(x => new RentVM(x))
                    .ToList();

                //populate categories select list 
                ViewBag.Categories = new SelectList(db.Rent_Category.ToList(), "Id", "Name");

                //set selected categorey
                ViewBag.SelectCategory = catId.ToString();
            }

            //set pagination

            ViewBag.OnePageOfProducts = ListOfProductVM.ToPagedList(pageNumber, 20);

            //return view with list
            return View(ListOfProductVM);
        }





        public ActionResult RentDetailsProduct(int Id)
        {

            //Declare DonateVM

            RentVM model;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //Get the product 

                Rent_Product product = db.Rent_Product.Find(Id);

                //make sure product exists
                if (product == null)
                {
                    return Content("That product does not exist");
                }

                //get Current user with product
                product.UserId = User.Identity.GetUserId();
                db.SaveChanges();

                //init model

                model = new RentVM(product);

                //make  a select list 

                model.Categories = new SelectList(db.Rent_Category.ToList(), "Id", "Name");

                //get all gallery images

                model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + Id + "/Gallery/Thumbs"))
                        .Select(fn => Path.GetFileName(fn));
            }
            return View(model);
        }
    }
}