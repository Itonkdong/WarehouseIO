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
    public class TransfersController : Controller
    {
        // GET: Transfer
        public ActionResult Index()
        {
            var (activeUser, db) = this.GetActiveUser();

            List<Warehouse> allUserWarehouses = activeUser.GetAllMyWarehouses(db, UserFetchOptions.IncludeTransfers);

            List<Transfer> allTransfers = new List<Transfer>(); 
                
                allUserWarehouses
                .ForEach(w =>
                {
                    allTransfers.AddRange(w.TransfersFromWarehouse
                        .Where(t => !allTransfers.Contains(t)));
                    allTransfers.AddRange(w.TransfersToWarehouse
                        .Where(t => !allTransfers.Contains(t)));

                });

            List<Transfer> transfersToAcceptOrReject = allTransfers
                .Where(t => t.MadeByUserId != activeUser.Id)
                .ToList();

            ManageTransfersViewModel model = new ManageTransfersViewModel()
            {
                AllTransfers = allTransfers,
                TransfersToAcceptOrReject = transfersToAcceptOrReject,
            };


            return View(model);
        }

        // GET: Transfer/Details/5
        public ActionResult DetailsTransferHistory(int? id)
        {
            return View();
        }

        public ActionResult DetailsTransferToAccept(int? id)
        {
            return View();
        }

        // GET: Transfer/Create
        public ActionResult Make(int fromWarehouseId, int? toWarehouseId)
        {
            (ApplicationUser activeUser, ApplicationDbContext db) = this.GetActiveUser();

            if (activeUser.Warehouses.Count < 2)
            {
                TempData["ErrorMessage"] = ErrorHandler.ErrorMessages.MUST_HAVE_AT_LEAST_TWO_WAREHOUSES;
                    
                return RedirectToAction("Add", "Warehouses");
            }

            Warehouse fromWarehouse = activeUser.GetWarehouse(fromWarehouseId);

            List<MovingItemViewModel> transferItems = fromWarehouse
                .StoredItems
                .Select(item => new MovingItemViewModel(item))
                .ToList();

            Warehouse toWarehouse = toWarehouseId switch
            {
                null => activeUser.Warehouses
                    .First(w => w.Id != fromWarehouseId),
                _ => activeUser.Warehouses
                    .First(w => w.Id == toWarehouseId)
            };


            MakeEditTransferViewModel model = new MakeEditTransferViewModel()
            {
                FromWarehouse = fromWarehouse,
                FromWarehouseId = fromWarehouse.Id,
                ToWarehouse = toWarehouse,
                ToWarehouseId = toWarehouse.Id,
                AllFromWarehouseItems = transferItems,
                AllWarehouses = activeUser.GetAllMyWarehouses(db)
            };


            return View(model);
        }

        // POST: Transfer/Create
        [HttpPost]
        public ActionResult Make(MakeEditTransferViewModel model)
        {

            var (activeUser, db) = this.GetActiveUser();
            Warehouse fromWarehouse = activeUser.GetWarehouse(model.FromWarehouseId);
            Warehouse toWarehouse = activeUser.GetWarehouse(model.ToWarehouseId);

            List<MovingItem> movingItems = model.AllFromWarehouseItems
                .Where(item => item.Included)
                .Where(item => item.TransferAmount != 0)
                .Select(item => new MovingItem(item))
                .ToList();

            if (movingItems.Count == 0)
            {
                TempData["ErrorMessage"] = ErrorHandler.ErrorMessages.TRANSFER_HAS_NO_ITEMS;
                return RedirectToAction("Make", "Transfers",
                    routeValues: new { fromWarehouseId = model.FromWarehouseId, toWarehouseId = model.ToWarehouseId });
            }

            Transfer transfer = new Transfer()
            {
                FromWarehouse = fromWarehouse,
                ToWarehouse = toWarehouse,
                MadeByUser = activeUser,
                MadeOn = DateTime.Now,
                Status = TransferStatus.StillPending,
                TransferItems = movingItems,
                ClosedOn = null
            };

            db.Transfers
                .Add(transfer);

            db.SaveChanges();

            return RedirectToAction("Index", "Transfers");

        }


        public ActionResult RejectAll()
        {
            throw new NotImplementedException();

            return RedirectToAction("Index", "Transfers");
        }

        public ActionResult AcceptAll()
        {
            throw new NotImplementedException();

            return RedirectToAction("Index", "Transfers");
        }

        public ActionResult DeleteAll()
        {
            throw new NotImplementedException();

            return RedirectToAction("Index", "Transfers");
        }




        // GET: Transfer/Edit/5
        public ActionResult Edit(int transferId)
        {
            throw new NotImplementedException();

            return View();
        }

        // POST: Transfer/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            throw new NotImplementedException();

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

        // GET: Transfer/Delete/5
        /*public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Transfer/Delete/5
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
