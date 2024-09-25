using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WarehouseIO.ControlClasses;
using WarehouseIO.Models;
using WarehouseIO.ViewModels;

namespace WarehouseIO.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        public ActionResult Index()
        {
            

            return View();
        }

        [Authorize]
        public ActionResult Dashboard(int? warehouseId)
        {
            var (activeUser, _) = this.GetActiveUser(this._db);

            List<Warehouse> allWarehouses = activeUser.GetAllMyWarehouses(this._db, UserFetchOptions.IncludeEverything);

            if (allWarehouses.Count == 0)
            {
                this.SetError("You must own at least one warehouse to view it the dashboard. Create one or become part of another.");
                return RedirectToAction("Add", "Warehouses");
            }

            Warehouse warehouse = warehouseId is null
                ? allWarehouses.FirstOrDefault()
                : allWarehouses.FirstOrDefault(w => w.Id == warehouseId);

            if (warehouse == null)
            {
                this.SetError("Warehouse is null.");
                return RedirectToAction("Dashboard", "Home");
            }

            warehouse.TransfersFromWarehouse = warehouse
                .TransfersFromWarehouse
                .OrderByDescending(w => w.MadeOn)
                .ToList();

            DashboardViewModel model = new DashboardViewModel
            {
                WarehouseId = warehouse.Id,
                Warehouse = warehouse,
                AllWarehouses = allWarehouses
            };

            return View(model);
        }
    }
}