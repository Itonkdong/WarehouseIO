using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Design;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using WarehouseIO.Controllers;

namespace WarehouseIO.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string? ProfileImage { get; set; }

        public DateTime DateJoined { get; set; }

        public virtual ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();

        [ForeignKey("MadeByUserId")]
        public virtual ICollection<Transfer> Transfers { get; set; } = new List<Transfer>();


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

        public static ApplicationUser GetUser(string email)
        {
            ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            return userManager.FindByEmail(email);
        }

        public Warehouse GetWarehouse(int warehouseId)
        {
            Warehouse warehouse = this
                .Warehouses
                .FirstOrDefault(warehouse => warehouse.Id == warehouseId);
            return warehouse;

        }

        public List<Warehouse> GetAllMyWarehouses(ApplicationDbContext db, UserFetchOptions options=UserFetchOptions.IncludeWarehouseStoredItems)
        {

            return options switch
            {
                UserFetchOptions.IncludeWarehouseStoredItems =>
                    this
                        .Warehouses
                        .Select(w =>
                        {
                            return db
                                .Warehouses
                                .Include(warehouse => warehouse.StoredItems)
                                .First(warehouse => warehouse.Id == w.Id);
                        })
                        .ToList(),
                UserFetchOptions.IncludeTransfers =>
                    this
                        .Warehouses
                        .Select(w =>
                        {
                            return db
                                .Warehouses
                                .Include(warehouse => warehouse.TransfersFromWarehouse)
                                .Include(warehouse => warehouse.TransfersToWarehouse)
                                .First(warehouse => warehouse.Id == w.Id);
                        })
                        .ToList(),
                UserFetchOptions.IncludeWarehouseStoredItemsAndTransfers =>
                    this
                        .Warehouses
                        .Select(w =>
                        {
                            return db
                                .Warehouses
                                .Include(warehouse => warehouse.StoredItems)
                                .Include(warehouse => warehouse.TransfersFromWarehouse)
                                .Include(warehouse => warehouse.TransfersToWarehouse)
                                .First(warehouse => warehouse.Id == w.Id);
                        })
                        .ToList(),
                UserFetchOptions.Default => this
                    .Warehouses
                    .ToList(),
                _ => throw new ArgumentOutOfRangeException(nameof(options), options, null)
            };
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<MovingItem> MovingItems { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<Shipment> Shipments { get; set; }



        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transfer>()
                .HasRequired(t => t.FromWarehouse)
                .WithMany(w => w.TransfersFromWarehouse)
                .HasForeignKey(t => t.FromWarehouseId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Transfer>()
                .HasRequired(t => t.ToWarehouse)
                .WithMany(w => w.TransfersToWarehouse)
                .HasForeignKey(t => t.ToWarehouseId)
                .WillCascadeOnDelete(false);


            base.OnModelCreating(modelBuilder);

        }
    }
}