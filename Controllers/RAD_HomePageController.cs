using R.A.D.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace R.A.D.Controllers
{
    public class RAD_HomePageController : Controller
    {
        // GET: RAD_HomePage
        public ActionResult HomePage()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                    ViewBag.AuctionProducts = db.Auction_Product.ToList();

            }
            return View();
        }
    }
}