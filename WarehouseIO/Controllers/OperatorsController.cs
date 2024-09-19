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
    [Authorize]
    public class OperatorsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        // GET: Operators
        public ActionResult Index(int? warehouseId)
        {
            var (activeUser, _) = this.GetActiveUser(this._db);

            List<Warehouse> allUserWarehouses = activeUser.GetAllMyWarehouses(this._db, UserFetchOptions.Default);

            if (allUserWarehouses.Count == 0)
            {
                this.SetError(ErrorHandler.ErrorMessages.MUST_HAVE_AT_LEAST_ONE_WAREHOUSES);
                return RedirectToAction("Add", "Warehouses");
            }


            Warehouse warehouse = warehouseId is null ? allUserWarehouses.First() : allUserWarehouses.FirstOrDefault(w => w.Id == warehouseId);

            if (warehouse is null)
            {
                this.SetError("Invalid warehouse id.");
                return RedirectToAction("Index", "Operators");
            }


            ManageOperatorsViewModel model = new ManageOperatorsViewModel
            {
                WarehouseId = warehouse.Id,
                Warehouse = warehouse,
                AllWarehouses = allUserWarehouses
            };

            return View(model);
        }

        public ActionResult Add(ManageWarehouseViewModel mode)
        {
            throw new NotImplementedException();
        }

        public ActionResult Remove(string email)
        {
            throw new NotImplementedException();
        }

       
    }
}
