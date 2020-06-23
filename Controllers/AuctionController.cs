using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.DataHandler.Encoder;
using PagedList;
using R.A.D.Models;
using R.A.D.Models.Auction;
using R.A.D.Models.ViewModels;
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
    public class AuctionController : Controller
    {
        public ActionResult ListOfAuctionProducts(int? page)
        {

            //declare a list of products of products vm
            List<Auction_Product> ListOfProductVM;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ListOfProductVM = db.Auction_Product.ToList();
                foreach( var item in ListOfProductVM)
                {
                    if (item.Auction_Ended == DateTime.Today)
                    {
                        db.Auction_Product.Remove(item);
                        db.SaveChanges();                       
                    }

                    if (item.LastPrice == null)
                    {
                        item.LastPrice = item.StartingPrice;
                        db.Auction_Product.Add(item);
                        
                    }
                }
                
               
            }          


            //set page number 

            var pageNumber = page ?? 1;


            ViewBag.OnePageOfProducts = ListOfProductVM.ToPagedList(pageNumber, 5);

            //return view with list
            return View(ListOfProductVM);
        }

        [HttpGet]
        public ActionResult Add_Auction_Product()
        {
            //Intial Model

            AuctionVM model = new AuctionVM();


            return View(model);
        }

        [HttpPost]
        public ActionResult Add_Auction_Product(AuctionVM model, HttpPostedFileBase file)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    return View(model);
                }
            }

            //declare product id 
            int id;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Auction_Product product = new Auction_Product();

                //product.ProductCode = model.ProductCode;
                product.Name = model.Name;
                product.Description = model.Description;
                product.CountryOfOrgin = model.CountryOfOrgin;
                product.Details = model.Details;
                product.StartingPrice = model.StartingPrice;               
                product.LastPrice = model.LastPrice;
                product.AdminId = User.Identity.GetUserId();
                product.Auction_Started = DateTime.Now;
                product.Auction_Ended = model.Auction_Ended;
                db.Auction_Product.Add(product);
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
                        ModelState.AddModelError("", "The Image was not uplaoded - wrong image extintion");
                        return View(model);
                    }
                }

                string ImageName = file.FileName;

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Auction_Product prod = db.Auction_Product.Find(id);
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

                image.Resize(200, 200);
                image.Save(path2);
            }
            #endregion
            //redirect
            return RedirectToAction("ListOfAuctionProducts");
        }
        public ActionResult DeleteProduct(int Id)
        {
            //Delete produts from DB

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Auction_Product product = db.Auction_Product.Find(Id);
                db.Auction_Product.Remove(product);
                db.SaveChanges();
            }

            //delete product folder
            var OriginalDirectorey = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

            var PathString = Path.Combine(OriginalDirectorey.ToString(), "Products\\" + Id.ToString());

            if (Directory.Exists(PathString))
                Directory.Delete(PathString, true);

            return RedirectToAction("ListOfAuctionProducts");
        }
        
        [HttpGet]
        public ActionResult UpdateProduct(int Id)
        {
            //Declare AuctionVM

            AuctionVM model;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //Get the product 

                Auction_Product product = db.Auction_Product.Find(Id);

                //make sure product exists
                if (product == null)
                {
                    return Content("This product does not exist");
                }

                //init model

                model = new AuctionVM(product);



                //get all gallery images

                model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + Id ))
                        .Select(fn => Path.GetFileName(fn));
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateProduct(AuctionVM model, HttpPostedFileBase file)
        {

            //get product id
            int id = model.Id;


            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id))
                              .Select(fn => Path.GetFileName(fn));


            //check model state 

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //make sure product name isunique

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (db.Auction_Product.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", " Product name is taken Please Enter Another One!");
                    return View(model);
                }
            }
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Auction_Product product = db.Auction_Product.Find(id);

                product.Name = model.Name;
                product.Description = model.Description;
                product.CountryOfOrgin = model.CountryOfOrgin;
                product.Details = model.Details;
                product.StartingPrice = model.StartingPrice;
                product.LastPrice = model.LastPrice;
                product.Auction_Started = DateTime.Now;
                product.Auction_Ended = model.Auction_Ended;



                if (model.ImageName != null)
                {
                    product.ImageName = model.ImageName;
                }

                db.SaveChanges();
            }

            //set tempdata message
            TempData["MM"] = "Product Updated Please Reload The Page ";


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
                    Auction_Product product = db.Auction_Product.Find(id);
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

                image.Resize(200, 200);
                image.Save(path2);
            }
            #endregion
            //Redirect 

            return RedirectToAction(nameof(ListOfAuctionProducts));
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

                    var PathString1 = Path.Combine(OriginalDirectorey.ToString(), "Products\\" + Id.ToString() );
                    var PathString2 = Path.Combine(OriginalDirectorey.ToString(), "Products\\" + Id.ToString() );



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
            string fullpath1 = Request.MapPath("~/Images/Uploads/Products" + "/" + Id.ToString()+"/" + imageName);
            string fullpath2 = Request.MapPath("~/Images/Uploads/Products" + "/" + Id.ToString()+"/" + imageName);

            if (System.IO.File.Exists(fullpath1))
                System.IO.File.Delete(fullpath1);

            if (System.IO.File.Exists(fullpath2))
                System.IO.File.Delete(fullpath2);

        }
    }
}

