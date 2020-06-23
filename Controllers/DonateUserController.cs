using PagedList;
using R.A.D.Models;
using RAD.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using R.A.D.Models.Donate;
using System.IO;
using Microsoft.AspNet.Identity;

namespace R.A.D.Controllers
{

    [Authorize(Roles = "User")]
    public class DonateUserController : Controller
    {
        // GET: DonateUser
        public ActionResult ProductsInCategories(int? page, int? catId = 0)
        {
            //declare a list of products of products vm
            List<DonationVM> ListOfProductVM;

            //set page number 

            var pageNumber = page ?? 1;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //init the list 
                ListOfProductVM = db.Donate_Product.ToArray()
                    .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                    .Select(x => new DonationVM(x))
                    .ToList();

                //populate categories select list 
                ViewBag.Categories = new SelectList(db.Donate_Category.ToList(), "Id", "Name");

                //set selected categorey
                ViewBag.SelectCategory = catId.ToString();
            }

            //set pagination

            ViewBag.OnePageOfProducts = ListOfProductVM.ToPagedList(pageNumber, 20);

            //return view with list
            return View(ListOfProductVM);
        }


        public ActionResult DetailsProduct(int Id)
        {
            //Declare DonateVM

            DonationVM model;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //Get the product 

                Donate_Product product = db.Donate_Product.Find(Id);

                //make sure product exists
                if (product == null)
                {
                    return Content("That product does not exist");
                }

                //get Current user with product
                product.UserId = User.Identity.GetUserId();
                db.SaveChanges();

                //init model

                model = new DonationVM(product);

                //make  a select list 

                model.Categories = new SelectList(db.Donate_Category.ToList(), "Id", "Name");

                //get all gallery images

                model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + Id + "/Gallery/Thumbs"))
                        .Select(fn => Path.GetFileName(fn));
            }
            return View(model);
        }
    }
}