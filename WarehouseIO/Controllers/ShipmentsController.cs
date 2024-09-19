using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WarehouseIO.ControlClasses;
using WarehouseIO.Models;
using WarehouseIO.ViewModels;

namespace WarehouseIO.Controllers
{
    [Authorize]
    public class ShipmentsController : Controller
    {

        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        // GET: Shipment
        public ActionResult Index()
        {
            return View();
        }

        // GET: Shipment/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Shipment/Create
        public ActionResult Make(int? warehouseId)
        {


            if (allWarehouses.Count <= 0)
            {
                this.SetError("You must own or be part of at least one warehouse to make a shipment. Create or become an operator in one.");
                return RedirectToAction("Add", "Warehouses");
            }

            Warehouse warehouse = warehouseId is null ? allWarehouses.First() : allWarehouses.First(w => w.Id == warehouseId);

            if (warehouse is null)
            {
                this.SetError("Warehouse is null");
                return RedirectToAction("Index", "Shipments");
            }


            warehouse = this._db
                .Warehouses
                .Include(w => w.StoredItems)
                .First(w => w.Id == warehouse.Id);

            MakeShipmentViewModel model = this.GetViewModel(warehouse);

            return View(model);
        }

        public MakeShipmentViewModel GetViewModel(Warehouse warehouse)
        {

            var (activeUser, _) = this.GetActiveUser(this._db);

            List<Warehouse> allWarehouses = activeUser.GetAllMyWarehouses(this._db, UserFetchOptions.Default);


            return new MakeShipmentViewModel
            {
                WarehouseId = warehouse.Id,
                Warehouse = warehouse,
                AllWarehouseItem = warehouse.GetMovingItemViewModels(),
                AllWarehouses = allWarehouses
            };
        }

        // POST: Shipment/Create
        [HttpPost]
        public ActionResult Make(MakeShipmentViewModel model)
        {
            Warehouse warehouse;
            if (!ModelState.IsValid)
            {
                this.SetError("Shipping Address is required");
                warehouse = this._db
                    .Warehouses
                    .Include(w => w.StoredItems)
                    .First(w => w.Id == model.WarehouseId);
                model = this.GetViewModel(warehouse);
                return View(model);
            }


            List<MovingItem> movingItems = model.AllWarehouseItem
                .Where(item => item.Included)
                .Where(item => item.TransferAmount != 0)
                .Select(item => new MovingItem(item))
                .ToList();

            if (movingItems.Count == 0)
            {
                this.SetError("A shipment must include at least one item.");
                return RedirectToAction("Make", "Shipments",
                    routeValues: new { warehouseId = model.WarehouseId});
            }

            warehouse = this._db
                .Warehouses
                .FirstOrDefault(w => w.Id == model.WarehouseId);

            if (warehouse == null)
            {
                this.SetError("Warehouse is null");
                return RedirectToAction("Make", "Shipments");
            }

            Shipment shipment = new Shipment
            {
                FromWarehouseId = warehouse.Id,
                FromWarehouse = warehouse,
                ShippingTo = model.ShippingTo,
                ShippingItems = movingItems,
                MadeOn = DateTime.Now
            };

            db.Transfers
                .Add(shipment);

            db.SaveChanges();



        }

        /*// GET: Shipment/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Shipment/Edit/5
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

        // GET: Shipment/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Shipment/Delete/5
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
