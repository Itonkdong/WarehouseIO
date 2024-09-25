using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using WarehouseIO.ControlClasses;
using WarehouseIO.Models;
using WarehouseIO.ViewModels;

namespace WarehouseIO.Controllers
{
    [Authorize]
    public class ShipmentsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public MakeShipmentViewModel GetMakeShipmentViewModel(Warehouse warehouse, ApplicationUser activeUser = null,
            List<Warehouse> allWarehouses = null)
        {
            if (activeUser is null)
            {
                var (activeUser1, _) = this.GetActiveUser(this._db);
                activeUser = activeUser1;
            }

            allWarehouses ??= activeUser.GetAllMyWarehouses(this._db, UserFetchOptions.Default);


            return new MakeShipmentViewModel
            {
                WarehouseId = warehouse.Id,
                Warehouse = warehouse,
                AllWarehouseItem = warehouse.GetMovingItemViewModels(),
                AllWarehouses = allWarehouses
            };
        }

        // GET: Shipment
        public ActionResult Index(int? warehouseId)
        {
            var (activeUser, _) = this.GetActiveUser(this._db);

            List<Warehouse> allWarehouses = activeUser.GetAllMyWarehouses(this._db, UserFetchOptions.IncludeShipments);

            if (allWarehouses.Count <= 0)
            {
                this.SetError("You must own or be part of at least one warehouse.");
                return RedirectToAction("Add", "Warehouses");
            }

            Warehouse warehouse = warehouseId is null
                ? allWarehouses.First()
                : allWarehouses.First(w => w.Id == warehouseId);


            if (warehouse is null)
            {
                this.SetError("Warehouse is null.");
                return RedirectToAction("Index", "Shipments");
            }

            ManageShipmentsViewModel model = new ManageShipmentsViewModel()
            {
                AllWarehouses = allWarehouses,
                Shipments = warehouse.Shipments.ToList(),
                Warehouse = warehouse,
                WarehouseId = warehouse.Id
            };

            return View(model);
        }

        // GET: Shipment/Details/5
        public ActionResult Details(int shipmentId)
        {
            Shipment shipment = this._db
                .Shipments
                .Include(s=>s.ShippingItems.Select(mi=>mi.Item))
                .Include(s=>s.FromWarehouse)
                .FirstOrDefault(s => s.Id == shipmentId);

            if (shipment is null)
            {
                this.SetError("Shipment is null.");
                return RedirectToAction("Index", "Shipments");
            }


            return View(shipment);
        }

        // GET: Shipment/Create
        public ActionResult Make(int? warehouseId)
        {
            var (activeUser, _) = this.GetActiveUser(this._db);
            List<Warehouse> allWarehouses = activeUser.GetAllMyWarehouses(this._db, UserFetchOptions.Default);

            if (allWarehouses.Count <= 0)
            {
                this.SetError(
                    "You must own or be part of at least one warehouse to make a shipment. Create or become an operator in one.");
                return RedirectToAction("Add", "Warehouses");
            }

            Warehouse warehouse = warehouseId is null
                ? allWarehouses.First()
                : allWarehouses.First(w => w.Id == warehouseId);

            if (warehouse is null)
            {
                this.SetError("Warehouse is null");
                return RedirectToAction("Index", "Shipments");
            }


            warehouse = this._db
                .Warehouses
                .Include(w => w.StoredItems)
                .First(w => w.Id == warehouse.Id);

            MakeShipmentViewModel model = this.GetMakeShipmentViewModel(warehouse, activeUser, allWarehouses);

            return View(model);
        }

        // POST: Shipment/Create
        [HttpPost]
        public ActionResult Make(MakeShipmentViewModel model)
        {
            Warehouse warehouse = this._db
                .Warehouses
                .Include(w => w.StoredItems)
                .First(w => w.Id == model.WarehouseId);

            if (warehouse == null)
            {
                this.SetError("Warehouse is null");
                return RedirectToAction("Make", "Shipments");
            }

            List<MovingItem> movingItems = model.AllWarehouseItem
                .Where(item => item.IncludeAll || item.TransferAmount != 0)
                .Select(movingItemViewModel =>
                {
                    if (movingItemViewModel.IncludeAll)
                    {
                        movingItemViewModel.TransferAmount = (int)movingItemViewModel.AvailableAmount;
                    }

                    return movingItemViewModel;
                })
                .Select(item => new MovingItem(item))
                .ToList();

            Shipment shipment = new Shipment
            {
                FromWarehouseId = warehouse.Id,
                FromWarehouse = warehouse,
                ShippingTo = model.ShippingTo,
                ShippingItems = movingItems,
                MadeOn = DateTime.Now
            };
            if (string.IsNullOrEmpty(model.ShippingTo))
            {
                this.SetError("Shipping Address is required.");
                model = this.GetMakeShipmentViewModel(warehouse);
                model.Shipment = shipment;
                return View(model);
            }


            if (movingItems.Count == 0)
            {
                this.SetError("A shipment must include at least one item.");
                model = this.GetMakeShipmentViewModel(warehouse);
                model.Shipment = shipment;
                return View(model);

            }

            TryResult tryResult = shipment.TryCommit(warehouse);

            if (!tryResult.Result)
            {
                this.SetError(tryResult.Exception!.Message);
                model = this.GetMakeShipmentViewModel(warehouse);
                model.Shipment = shipment;
                return View(model);
            }

            this._db.Shipments
                .Add(shipment);

            this._db.SaveChanges();

            return RedirectToAction("Index", "Shipments", routeValues: new {warehouseId = model.WarehouseId});
        }


        public TryResult TryCancel(Shipment shipment)
        {

            try
            {
                if (shipment is null)
                {
                    return new TryResult(false, new Exception("Shipment does not exist."));
                }

                foreach (MovingItem movingItem in shipment.ShippingItems)
                {
                    movingItem.Item.Amount += movingItem.Amount;
                }

                this
                    ._db
                    .Shipments
                    .Remove(shipment);

                this._db.SaveChanges();
            }
            catch (Exception e)
            {
                return new TryResult(false, e);
            }

            return new TryResult(true, null);
        }

        public ActionResult Cancel(int shipmentId)
        {
            Shipment shipment = this
                ._db
                .Shipments.Include(shipment => shipment.ShippingItems.Select(movingItem => movingItem.Item))
                .Include(shipment => shipment.FromWarehouse)
                .FirstOrDefault(s => s.Id == shipmentId);

            if (shipment is null)
            {
                this.SetError("Shipment is null.");
                return this.RedirectToAction("Index", "Shipments", routeValues: null);
            }

            TryResult tryResult = this.TryCancel(shipment);

            if (!tryResult.Result)
            {
                this.SetError(tryResult.Exception!.Message);
            }


            return this.RedirectToAction("Index", "Shipments", routeValues: new { warehouseId = shipment.FromWarehouse.Id });
        }

        public ActionResult CancelAll(int warehouseId)
        {
            Warehouse warehouse = this
                ._db
                .Warehouses.Include(warehouse => warehouse.Shipments
                    .Select(shipment => shipment.ShippingItems
                        .Select(movingItem => movingItem.Item)))
                .FirstOrDefault(w => w.Id == warehouseId);

            if (warehouse is null)
            {
                this.SetError("Warehouse is null.");
                return this.RedirectToAction("Index", "Shipments");
            }


            List<Shipment> shipmentsToCancel = warehouse.Shipments
                .Where(s => s.Status == ShipmentStatus.Preparing)
                .ToList();

            if (shipmentsToCancel.Count == 0)
            {
                this.SetError("You do not have any shipments to cancel.");
                return RedirectToAction("Index", "Shipments", routeValues: new { warehouseId = warehouseId });
            }

            foreach (Shipment shipment in shipmentsToCancel)
            {
                TryResult tryResult = shipment.TryCancel(this._db);

                if (!tryResult.Result)
                {
                    this.SetError("Some shipments fail to cancel");
                }
            }

            return RedirectToAction("Index", "Shipments", routeValues: new { warehouseId = warehouseId });
        }

        public ActionResult Finalize(int shipmentId)
        {
            Shipment shipment = this
                ._db
                .Shipments
                .Include(s => s.FromWarehouse)
                .Include(s => s.ShippingItems)
                .FirstOrDefault(s => s.Id == shipmentId);

            if (shipment is null)
            {
                this.SetError("Shipment is null.");
                return this.RedirectToAction("Index", "Shipments", routeValues: null);
            }

            TryResult tryResult = this.TryFinalize(shipment);

            if (!tryResult.Result)
            {
                this.SetError(tryResult.Exception!.Message);
            }


            return this.RedirectToAction("Index", "Shipments", routeValues: new { warehouseId = shipment.FromWarehouseId });
        }

        public TryResult TryFinalize(Shipment shipment)
        {
            if (shipment is null)
            {
                return new TryResult(false, new Exception("Shipment is null."));
            }

            try
            {
                shipment.Finalize(shipment.FromWarehouse);
                this._db.SaveChanges();
            }
            catch (Exception e)
            {
                return new TryResult(false, e);
            }

            return new TryResult(true, null);

        }

        public ActionResult FinalizeAll(int warehouseId)
        {

            Warehouse warehouse = this
                ._db
                .Warehouses.Include(warehouse => warehouse.Shipments
                    .Select(shipment => shipment.ShippingItems))
                .FirstOrDefault(w => w.Id == warehouseId);

            if (warehouse is null)
            {
                this.SetError("Warehouse is null.");
                return this.RedirectToAction("Index", "Shipments");
            }


            List<Shipment> shipmentsToFinalize = warehouse.Shipments
                .Where(s => s.Status == ShipmentStatus.Preparing)
                .ToList();

            if (shipmentsToFinalize.Count == 0)
            {
                this.SetError("You do not have any shipments to finalize.");
                return RedirectToAction("Index", "Shipments", routeValues: new { warehouseId = warehouseId });
            }

            foreach (Shipment shipment in shipmentsToFinalize)
            {
                TryResult tryResult = this.TryFinalize(shipment);

                if (!tryResult.Result)
                {
                    this.SetError("Some shipments fail to finalize.");
                }
            }

            return RedirectToAction("Index", "Shipments", routeValues: new { warehouseId = warehouseId });
        }
    }
}