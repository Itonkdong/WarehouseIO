using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WarehouseIO.Models;

namespace WarehouseIO.ControlClasses
{
    public static class ControllerExtender
    {
        public static (ApplicationUser, ApplicationDbContext) GetActiveUser(this Controller controller, ApplicationDbContext db = null)
        {
            db = db ?? new ApplicationDbContext();
            ApplicationUser activeUser = db
                .Users
                .First(user => user.Email == controller.User.Identity.Name);
            return (activeUser, db);
        }

        public static (Warehouse, ApplicationDbContext) GetWarehouse(this Controller controller, int warehouseId, ApplicationDbContext db = null)
        {
            db = db ?? new ApplicationDbContext();
            Warehouse warehouse = db
                .Warehouses
                .FirstOrDefault(warehouse => warehouse.Id == warehouseId);
            return (warehouse,db);
        }

        public static Warehouse GetWarehouseFromUser(this Controller controller, int warehouseId, ApplicationUser user)
        {
            Warehouse warehouse = user
                .Warehouses
                .FirstOrDefault(warehouse => warehouse.Id == warehouseId);
            return warehouse;
        }

        public static (Item, ApplicationDbContext) GetItem(this Controller controller , int itemId, Warehouse warehouse, ApplicationDbContext db = null)
        {
            db = db ?? new ApplicationDbContext();
            Item item = warehouse
                .StoredItems
                .FirstOrDefault(item => item.Id == itemId);
            return (item,db);
        }


        public static void SetLocalError(this Controller controller, string errorMessage)
        {
            controller.ViewBag["ErrorMessage"] = errorMessage;
        }

        public static void SetError(this Controller controller, string errorMessage)
        {
            controller.TempData["ErrorMessage"] = errorMessage;
        }

        public static string GetError(this Controller controller)
        {
            return (string) controller.TempData["ErrorMessage"];
        }

        public static string GetLocalError(this Controller controller)
        {
            return (string) controller.ViewBag["ErrorMessage"];
        }


    }
}