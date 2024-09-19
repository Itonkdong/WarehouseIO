using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Helpers;
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

        // TODO: TEMPORARY, REMOVE THIS
        public ActionResult AddOperatorToManagers(string email, int warehouseId)
        {
            Warehouse warehouse = this._db
                .Warehouses
                .FirstOrDefault(w => w.Id == warehouseId);

            ApplicationUser user = this._db
                .Users
                .FirstOrDefault(u => u.Email == email);

            warehouse
                .Managers
                .Add(user);

            this._db.SaveChanges();

            return RedirectToAction("Index", "Operators");
        }

        public ActionResult Add(ManageOperatorsViewModel model)
        {
            ApplicationUser user = this._db
            .Users
                .FirstOrDefault(u => u.Email == model.NewOperatorEmail);

            if (user is null)
            {
                this.SetError($"There is no user with the email: {model.NewOperatorEmail}.");
                return RedirectToAction("Index", "Operators" , routeValues: new {warehouseId = model.WarehouseId});
            }

            Warehouse warehouse = this._db.Warehouses
                .Include(warehouse => warehouse.Operators)
                .FirstOrDefault(w => w.Id == model.WarehouseId);

            if (warehouse is null)
            {
                this.SetError($"Warehouse with id: {model.WarehouseId} does not exist anymore.");
                return RedirectToAction("Index", "Operators");

            }

            if (warehouse.Operators.Any(o=>o.Email == user.Email))
            {
                this.SetError($"User: {model.NewOperatorEmail} is already an operator of the warehouse.");
                return RedirectToAction("Index", "Operators", routeValues: new { warehouseId = model.WarehouseId });
            }

            warehouse.Operators.Add(user);

            this._db.SaveChanges();

            return RedirectToAction("Index", "Operators", routeValues: new { warehouseId = model.WarehouseId });
        }

        public ActionResult Remove(string email, int warehouseId)
        {

            Warehouse warehouse = this._db
                .Warehouses
                .Include(warehouse => warehouse.Managers)
                .Include(warehouse => warehouse.Operators)
                .FirstOrDefault(w => w.Id == warehouseId);

            if (warehouse is null)
            {
                this.SetError("Warehouse does not exits.");
                return RedirectToAction("Index", "Operators", routeValues: new { warehouseId = warehouseId });
            }

            ApplicationUser user = this._db
                .Users
                .FirstOrDefault(u => u.Email == email);

            if (user is null)
            {
                this.SetError("User does not exits.");
                return RedirectToAction("Index", "Operators", routeValues: new { warehouseId = warehouseId });

            }

            if (!warehouse.Operators.Any(o=>o.Id == user.Id))
            {
                this.SetError("User is not an operator in the warehouse.");
                return RedirectToAction("Index", "Operators", routeValues: new { warehouseId = warehouseId });
            }


            if (warehouse.Operators.Count == 1)
            {
                this.SetError("Only one more operator left in this warehouse. A warehouse must have at least one operator.");
                return RedirectToAction("Index", "Operators", routeValues: new { warehouseId = warehouseId });
            }

            warehouse.Operators.Remove(user);

            if (warehouse.Managers.Any(m=>m.Id == user.Id))
            {
                warehouse.Managers.Remove(user);

            }

            this._db.SaveChanges();
            return RedirectToAction("Index", "Operators");


        }

        public ActionResult RemoveAll(string email)
        {
            throw new NotImplementedException();
        }

        public ActionResult Promote(string email, int warehouseId)
        {
            Warehouse warehouse = this._db
                .Warehouses
                .Include(warehouse => warehouse.Managers)
                .FirstOrDefault(w => w.Id == warehouseId);

            if (warehouse is null)
            {
                this.SetError("Warehouse does not exits.");
                return RedirectToAction("Index", "Operators", routeValues: new { warehouseId = warehouseId });
            }

            ApplicationUser user = this._db
                .Users
                .FirstOrDefault(u => u.Email == email);

            if (user is null)
            {
                this.SetError("User does not exits.");
                return RedirectToAction("Index", "Operators", routeValues: new { warehouseId = warehouseId });

            }

            if (warehouse.Managers.Any(m=>m.Email == email))
            {
                this.SetError("User is already manager of the warehouse.");
                return RedirectToAction("Index", "Operators", routeValues: new { warehouseId = warehouseId });
            }

            warehouse
                .Managers
                .Add(user);

            this._db.SaveChanges();

            return RedirectToAction("Index", "Operators");
        }

        public ActionResult Demote(string email, int warehouseId)
        {
            Warehouse warehouse = this._db
                .Warehouses
                .Include(warehouse => warehouse.Managers)
                .FirstOrDefault(w => w.Id == warehouseId);

            if (warehouse is null)
            {
                this.SetError("Warehouse does not exits.");
                return RedirectToAction("Index", "Operators", routeValues: new { warehouseId = warehouseId });
            }

            ApplicationUser user = this._db
                .Users
                .FirstOrDefault(u => u.Email == email);

            if (user is null)
            {
                this.SetError("User does not exits.");
                return RedirectToAction("Index", "Operators", routeValues: new { warehouseId = warehouseId });

            }

            if (!warehouse.Managers.Any(m => m.Email == email))
            {
                this.SetError("User is not a manager of the warehouse.");
                return RedirectToAction("Index", "Operators", routeValues: new { warehouseId = warehouseId });
            }

            if (warehouse.Managers.Count <= 1)
            {
                this.SetError("Only one more manger left in this warehouse. A warehouse must have at least one manger.");
                return RedirectToAction("Index", "Operators", routeValues: new { warehouseId = warehouseId });
            }

            warehouse
                .Managers
                .Remove(user);

            this._db.SaveChanges();

            return RedirectToAction("Index", "Operators");
        }
    }
}
