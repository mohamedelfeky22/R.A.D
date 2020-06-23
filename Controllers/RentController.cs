using Microsoft.AspNet.Identity;
using PagedList;
using R.A.D.Models;
using R.A.D.Models.Donate;
using R.A.D.Models.Rent;
using R.A.D.Models.ViewModels;
using RAD.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace R.A.D.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RentController : Controller
    {
        public ActionResult RentProducts(int? page, int? catId = 0)
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

        [HttpGet]
        public ActionResult Add_Rent_Product()
        {
            //Intial Model

            RentVM model = new RentVM();
            //add selet list to categorey models
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                model.Categories = new SelectList(db.Rent_Category.ToList(), "Id", "Name");
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Add_Rent_Product(RentVM model, HttpPostedFileBase file)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    model.Categories = new SelectList(db.Rent_Category.ToList(), "Id", "Name");
                    return View(model);
                }
            }
            //declare product id 
            int id;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Rent_Product product = new Rent_Product();
                product.Name = model.Name;
                product.Price = model.Price;
                product.Description = model.Description;
                product.CategoryId = model.CategoryId;
                product.Rent_Started = DateTime.Now;
                product.Renting_Period = model.Renting_Period;
                product.AdminId = User.Identity.GetUserId();


                db.Rent_Product.Add(product);
                db.SaveChanges();

                //get id 
                id = product.Id;

            }

            TempData["SM"] = "You have addedd a product sussecfully";


            #region
            //Create necessary directiories

            var OriginalDirectorey = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

            var PathString1 = Path.Combine(OriginalDirectorey.ToString(), "Products");
            var PathString2 = Path.Combine(OriginalDirectorey.ToString(), "Products\\" + id.ToString());
            var PathString3 = Path.Combine(OriginalDirectorey.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var PathString4 = Path.Combine(OriginalDirectorey.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var PathString5 = Path.Combine(OriginalDirectorey.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");



            if (!Directory.Exists(PathString1))
                Directory.CreateDirectory(PathString1);


            if (!Directory.Exists(PathString2))
                Directory.CreateDirectory(PathString2);


            if (!Directory.Exists(PathString3))
                Directory.CreateDirectory(PathString3);


            if (!Directory.Exists(PathString4))
                Directory.CreateDirectory(PathString4);


            if (!Directory.Exists(PathString5))
                Directory.CreateDirectory(PathString5);


            //Create if a file was upload 
            if (file != null && file.ContentLength > 0)
            {

                //Get file Extention
                string ext = file.ContentType.ToLower();


                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        model.Categories = new SelectList(db.Donate_Category.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The Image was not uplaoded - wrong image extintion");
                        return View(model);
                    }
                }

                string ImageName = file.FileName;

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Rent_Product prod = db.Rent_Product.Find(id);
                    prod.ImageName = ImageName;
                    db.SaveChanges();

                }

                //set original and thumb image pathss
                var path = string.Format("{0}\\{1}", PathString2, ImageName);
                var path2 = string.Format("{0}\\{1}", PathString3, ImageName);

                //save original
                file.SaveAs(path);


                //create and save thumbs 
                WebImage image = new WebImage(file.InputStream);

                image.Resize(400, 400);
                image.Save(path2);
            }

            #endregion
            //redirect

            return RedirectToAction("Add_Rent_Product");
        }

        [HttpGet]
        public ActionResult UpdateProduct(int Id)
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

        [HttpPost]
        public ActionResult UpdateProduct(RentVM model, HttpPostedFileBase file)
        {

            //get product id
            int id = model.Id;

            //populate categories selectlist and gallery images
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                model.Categories = new SelectList(db.Rent_Category.ToList(), "Id", "Name");
            }
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                              .Select(fn => Path.GetFileName(fn));


            //check model state 

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //make sure product name isunique

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (db.Rent_Product.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "That product name is tacken!");
                    return View(model);
                }
            }
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Rent_Product product = db.Rent_Product.Find(id);

                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.Renting_Period = model.Renting_Period;
                product.CategoryId = model.CategoryId;
                if (model.ImageName != null)
                {
                    product.ImageName = model.ImageName;
                }

                db.SaveChanges();
            }

            //set tempdata message
            TempData["SM"] = "You have Updated a product sussecfully";


            #region Image Upload 
            //Create if a file was upload 
            if (file != null && file.ContentLength > 0)
            {

                //Get file Extention
                string ext = file.ContentType.ToLower();


                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        model.Categories = new SelectList(db.Donate_Category.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The Image was not uplaoded - wrong image extintion");
                        return View(model);
                    }
                }


                var OriginalDirectorey = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

                var PathString1 = Path.Combine(OriginalDirectorey.ToString(), "Products\\" + id.ToString());
                var PathString2 = Path.Combine(OriginalDirectorey.ToString(), "Products\\" + id.ToString() + "\\Thumbs");


                //Delete files from directories

                DirectoryInfo di1 = new DirectoryInfo(PathString1);
                DirectoryInfo di2 = new DirectoryInfo(PathString2);

                foreach (FileInfo file1 in di1.GetFiles())
                    file1.Delete();

                foreach (FileInfo file2 in di2.GetFiles())
                    file2.Delete();


                //save images name 

                string imageName = file.FileName;

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Rent_Product product = db.Rent_Product.Find(id);
                    product.ImageName = imageName;
                    db.SaveChanges();
                }
                //save original and thumb images

                var path = string.Format("{0}\\{1}", PathString1, imageName);
                var path2 = string.Format("{0}\\{1}", PathString2, imageName);

                //save original
                file.SaveAs(path);


                //create and save thumbs 
                WebImage image = new WebImage(file.InputStream);

                image.Resize(400, 400);
                image.Save(path2);
            }


            #endregion

            //Redirect 

            return RedirectToAction(nameof(UpdateProduct));

        }

        public ActionResult DeleteProduct(int Id)
        {
            //Delete produts from DB

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Rent_Product product = db.Rent_Product.Find(Id);
                db.Rent_Product.Remove(product);
                db.SaveChanges();
            }

            //delete product folder
            var OriginalDirectorey = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

            var PathString = Path.Combine(OriginalDirectorey.ToString(), "Products\\" + Id.ToString());

            if (Directory.Exists(PathString))

                Directory.Delete(PathString, true);

            return RedirectToAction("RentProducts");
        }

        [HttpPost]
        public void SaveGalleryImages(int? Id)
        {
            //loop through files

            foreach (string fileName in Request.Files)
            {
                //innt  the file
                HttpPostedFileBase file = Request.Files[fileName];
                //check it`s not null

                if (file != null && file.ContentLength > 0)
                {
                    //Create necessary directiories

                    var OriginalDirectorey = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

                    var PathString1 = Path.Combine(OriginalDirectorey.ToString(), "Products\\" + Id.ToString() + "\\Gallery");
                    var PathString2 = Path.Combine(OriginalDirectorey.ToString(), "Products\\" + Id.ToString() + "\\Gallery\\Thumbs");



                    //set original and thumb image pathss
                    var path = string.Format("{0}\\{1}", PathString1, file.FileName);
                    var path2 = string.Format("{0}\\{1}", PathString2, file.FileName);

                    //save original
                    file.SaveAs(path);

                    //create and save thumbs 
                    WebImage image = new WebImage(file.InputStream);

                    image.Resize(400, 400);
                    image.Save(path2);
                }
            }
        }

        [HttpPost]
        public void DeleteGalleryImages(int Id, string imageName)
        {
            string fullpath1 = Request.MapPath("~/Images/Uploads/Products" + Id.ToString() + "/Gallery/" + imageName);
            string fullpath2 = Request.MapPath("~/Images/Uploads/Products" + Id.ToString() + "/Gallery/Thumbs/" + imageName);

            if (System.IO.File.Exists(fullpath1))
                    System.IO.File.Delete(fullpath1);

            if (System.IO.File.Exists(fullpath2))
                 System.IO.File.Delete(fullpath2);

        }
    }
}














