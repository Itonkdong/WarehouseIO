using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WarehouseIO.ControlClasses;
using WarehouseIO.Models;
using WarehouseIO.ViewModels;

namespace WarehouseIO.Controllers
{
    [Authorize]
    public class WarehousesController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();


        private ApplicationUser GetActiveUser()
        {
            return this._db
                .Users
                .FirstOrDefault(user => user.Email == User.Identity.Name);
        }

        private Warehouse GetWarehouse(int warehouseId)
        {
            return this._db
                .Warehouses
                .FirstOrDefault(warehouse => warehouse.Id == warehouseId);
        }

        // GET: Warehouse
        public ActionResult Index(int? warehouseId)
        {
            ApplicationUser activeUser = this.GetActiveUser();

            if (activeUser.Warehouses.Count == 0)
            {
                this.SetError("You must have at least one warehouse. Create one or become operator of another.");
                return RedirectToAction("Add", "Warehouses", routeValues: null);
            }

            List<Warehouse> allWarehouses = activeUser
                .Warehouses
                .Select(w =>
                {
                    return this._db
                        .Warehouses
                        .Include(warehouse => warehouse.StoredItems)
                        .First(warehouse => warehouse.Id == w.Id);
                })
                .ToList();

            Warehouse warehouse;
            ManageWarehouseViewModel model;

            if (warehouseId is null)
            {
                warehouse = activeUser.Warehouses.First();
                model = new ManageWarehouseViewModel(warehouse)
                {
                    AllWarehouses = allWarehouses
                };
                return View(model);
            }

            warehouse = activeUser
                .Warehouses
                .FirstOrDefault(warehouse1 => warehouse1.Id == warehouseId);

            if (warehouse is null)
            {
                return RedirectToAction("Index", "Warehouses");
            }
            model = new ManageWarehouseViewModel(warehouse)
            {
                AllWarehouses = allWarehouses
            };

            return View(model);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(Warehouse model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser activeUser = this.GetActiveUser();
            model.Operators.Add(activeUser);
            model.Managers.Add(activeUser);
            this._db.Warehouses.Add(model);

            this._db.SaveChanges();

            Warehouse newWarehouseEntry = this._db
                .Warehouses
                .First(warehouse => warehouse.Name == model.Name && warehouse.Location == model.Location);

            return RedirectToAction("Index", "Warehouses", routeValues: new {warehouseId = newWarehouseEntry.Id});
        }


        // GET: Warehouse/Edit/5
        public ActionResult Edit(int warehouseId)
        {
            Warehouse warehouse = this.GetWarehouse(warehouseId);

            if (warehouse is null)
            {
                this.SetError("Warehouse is null. Create one or become operator of another.");
                return this.RedirectToAction("Add", "Warehouses");
            }

            return View(warehouse);
        }

        [HttpPost]
        public ActionResult Edit(Warehouse model)
        {
            Warehouse warehouse = this.GetWarehouse(model.Id);

            if (!warehouse.TryUpdate(model))
            {
                return RedirectToAction("Index", "Warehouses", routeValues: new { warehouseId = model.Id });
            }

            if (!warehouse.HasEnoughSpace())
            {
                long currentCapacity = warehouse.CurrentCapacity;
                ViewBag.ErrorMessage =
                    $"Invalid max capacity. The current capacity: {currentCapacity}, is bigger then the one you have set: {model.MaxCapacity}.";
                return View(model);
            }

            this._db
                .Entry(warehouse)
                .State = EntityState.Modified;

            this._db.SaveChanges();

            return RedirectToAction("Index", "Warehouses", routeValues: new { warehouseId = model.Id }); ;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
