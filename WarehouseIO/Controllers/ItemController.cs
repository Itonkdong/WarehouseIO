using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WarehouseIO.ControlClasses;
using WarehouseIO.Exceptions;
using WarehouseIO.Models;
using WarehouseIO.ViewModels;

namespace WarehouseIO.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        // GET: Item/Create

        private readonly ApplicationDbContext _db = new ApplicationDbContext();


        private Warehouse GetWarehouse(int warehouseId)
        {
            return this._db
                .Warehouses
                .FirstOrDefault(warehouse => warehouse.Id == warehouseId);
        }

        private Item GetItem(int itemId, Warehouse warehouse)
        {
            return warehouse
                .StoredItems
                .FirstOrDefault(item => item.Id == itemId);
        }

        private AddEditItemViewModel MakeAddEditItemViewModel(Warehouse warehouse, Item? item=null)
        {
            List<ItemType> itemTypes = EnumHandler.GetAllEnumValues<ItemType>();


            if (item != null)
            {
                return new AddEditItemViewModel(warehouse, item)
                {
                    AllItemTypes = itemTypes
                };
            }

            return new AddEditItemViewModel(warehouse)
            {
                AllItemTypes = itemTypes
            };
        }
        public ActionResult Add(int warehouseId)
        {
            Warehouse? warehouse = this.GetWarehouse(warehouseId);

            if (warehouse is null)
            {
                this.SetError("Warehouse is null. Create another one.");
                return RedirectToAction("Add", "Warehouses");
            }

            AddEditItemViewModel model = this.MakeAddEditItemViewModel(warehouse);

            return View(model);
        }

        // POST: Item/Create
        [HttpPost]
        public ActionResult Add(AddEditItemViewModel model)
        {
            Warehouse warehouse = this.GetWarehouse(model.WarehouseId);
            if (!ModelState.IsValid)
            {
                model = this.MakeAddEditItemViewModel(warehouse);
                return View(model);
            }


            Item item = new Item
            {
                Name = model.Name,
                Description = model.Description,
                Type = model.Type,
                Size = model.Size,
                EstPrice = model.Size,
                Amount = model.Amount,
                ImageUrl = model.ImageUrl,
                WarehouseId = warehouse.Id,
                Warehouse = warehouse
            };

            if (item.Size == 0)
            {
                this.SetError("An item size can not be zero.");
                model = this.MakeAddEditItemViewModel(warehouse, item);
                return View(model);
            }

            this._db
                .Items
                .Add(item);

            this._db
                .SaveChanges();

            return RedirectToAction("Index", "Warehouses", routeValues: new { warehouseId= model.WarehouseId });

        }

        // GET: Item/Edit/5
        public ActionResult Edit(int warehouseId, int itemId)
        {
            Warehouse warehouse = this.GetWarehouse(warehouseId);

            if (warehouse is null)
            {
                this.SetError("Warehouse is null. Create another one.");
                return RedirectToAction("Add", "Warehouses");
            }

            Item item = this.GetItem(itemId, warehouse);

            if (item is null)
            {
                this.SetError("Item is null. Create one.");
                return RedirectToAction("Add", "Item", routeValues: new {warehouseId = warehouseId});
            }

            AddEditItemViewModel model = this.MakeAddEditItemViewModel(warehouse, item);

            return View(model);
        }

        

        // POST: Item/Edit/5
        [HttpPost]
        public ActionResult Edit(AddEditItemViewModel model)
        {
            Warehouse warehouse = this.GetWarehouse(model.WarehouseId);
            Item item = this.GetItem(model.Id, warehouse);

            if (warehouse is null)
            {
                this.SetError("Warehouse is null. Create another one.");
                return RedirectToAction("Add", "Warehouses");
            }

            if (item is null)
            {
                this.SetError("Item is null. Create one.");
                return RedirectToAction("Add", "Item", routeValues: new { warehouseId = warehouse.Id });
            }

            if (!ModelState.IsValid)
            {
                model = this.MakeAddEditItemViewModel(warehouse, item);
                return View(model);
            }


            Item updatedItem = new Item
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Type = model.Type,
                Size = model.Size,
                EstPrice = model.EstPrice,
                Amount = model.Amount,
                ImageUrl = model.ImageUrl,
                WarehouseId = warehouse.Id,
                Warehouse = warehouse
            };

            if (updatedItem.Size == 0)
            {
                this.SetError("An item size can not be zero.");
                model = this.MakeAddEditItemViewModel(warehouse, item);
                return View(model);
            }


            try
            {
                bool result = item.TryUpdate(updatedItem);
                if (!result)
                {
                    return RedirectToAction("Index", "Warehouses", routeValues: new { warehouseId = model.WarehouseId });
                }
            }
            catch (InvalidAmountValueException e)
            {
                model = this.MakeAddEditItemViewModel(warehouse, item);
                model.ErrorMessage = e.Message;
                return View(model);
            }

            if (!warehouse.HasEnoughSpace())
            {
                model = this.MakeAddEditItemViewModel(warehouse, item);
                model.ErrorMessage = $"There is not enough space in the warehouse for that amount of that item.";
                return View(model);
            }

            this._db
                .Entry(item)
                .State = EntityState.Modified;

            this._db
                .SaveChanges();


            return RedirectToAction("Index", "Warehouses", routeValues: new { warehouseId = model.WarehouseId });

        }

        public ActionResult Remove(int itemId, int warehouseId)
        {
            Item item = this._db
                .Items
                .FirstOrDefault(i => i.Id == itemId);

            if (item is null)
            {
                this.SetError("Item is null.");

                return RedirectToAction("Index", "Warehouses", routeValues: new {warehouseId = warehouseId});
            }

            item.Warehouse = null;
            item.WarehouseId = null;

            this._db.Entry(item).State = EntityState.Modified;
            this._db.SaveChanges();

            return RedirectToAction("Index", "Warehouses", routeValues: new { warehouseId = warehouseId });


        }
    }
}
