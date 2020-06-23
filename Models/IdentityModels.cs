using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using R.A.D.Models.Auction;
//using R.A.D.Models.Auction;
using R.A.D.Models.Donate;
using R.A.D.Models.Rent;

namespace R.A.D.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public string Name { get; set; }
        public string Photo { get; set; }

        //public virtual List<Donate_Product> Donate_Products { get; set; }
        //public virtual List<Auction_Product> Auction_Product { get; set; }
        //public virtual List<Rent_Product> Rent_Product { get; set; }

        
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public virtual DbSet<Donate_Product> Donate_Product { get; set; }
        public virtual DbSet<Donate_Category> Donate_Category { get; set; }

        public virtual DbSet<Auction_Product> Auction_Product { get; set; }

        public virtual DbSet<Rent_Category> Rent_Category { get; set; }
        public virtual DbSet<Rent_Product> Rent_Product { get; set; }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }


    }
}