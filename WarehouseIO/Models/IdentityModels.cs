using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WarehouseIO.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        // [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; }

        // [Column(TypeName = "nvarchar(500)")]
        public string? ProfileImage { get; set; }

        public DateTime DateJoined { get; set; }

        /*public virtual ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
        public virtual ICollection<Transfer> Transfers { get; set; } = new List<Transfer>();*/


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public void UpdateUser(AccountInformationViewModel model)
        {
            this.Name = model.Name;
            this.UserName = model.Email;
            this.Email = model.Email;
            this.ProfileImage = model.ProfileImage;
        }

        public static bool IsEmailValid(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            using ApplicationDbContext context = new ApplicationDbContext();
            ApplicationUser user = context
                .Users
                .FirstOrDefault(user => user.Email == email);

            return user == null;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        /*public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<MovingItem> MovingItems { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<Shipment> Shipments { get; set; }*/



        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        /*protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            /*modelBuilder.Entity<Transfer>()
                .HasRequired(t => t.FromWarehouse)
                .WithMany(w => w.TransfersFromWarehouse)
                .HasForeignKey(t => t.FromWarehouseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Transfer>()
                .HasRequired(t => t.ToWarehouse)
                .WithMany(w => w.TransfersToWarehouse)
                .HasForeignKey(t => t.ToWarehouseId)
                .WillCascadeOnDelete(false); #1#

        }*/
    }
}