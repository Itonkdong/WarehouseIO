using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
                .First(user => user.Email == User.Identity.Name);
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
                .First(warehouse1 => warehouse1.Id == warehouseId);
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

        /*// POST: Warehouse/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Warehouse/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Warehouse/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }*/
    }
}
