using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WarehouseIO.ControlClasses;
using WarehouseIO.Models;
using WarehouseIO.ViewModels;
using WebGrease.Css.Extensions;

namespace WarehouseIO.Controllers
{
    [Authorize]
    public class TransfersController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();


        public List<Transfer> GetAllTransfers(List<Warehouse> allUserWarehouses)
        {
            List<Transfer> allTransfers = new List<Transfer>();

            allUserWarehouses
                .ForEach(w =>
                {
                    allTransfers.AddRange(w.TransfersFromWarehouse
                        .Where(t => !allTransfers.Contains(t)));
                    allTransfers.AddRange(w.TransfersToWarehouse
                        .Where(t => !allTransfers.Contains(t)));
                });

            return allTransfers;
        }

        public List<Transfer> GetAllPendingTransfers(List<Transfer> allTransfers,List<Warehouse> allUserWarehouses,ApplicationUser activeUser= null)
        {
            activeUser = activeUser ?? this.GetActiveUser().Item1;

            return allTransfers
                .Where(t => t.MadeByUserId != activeUser.Id)
                .Where(t => t.Status == TransferStatus.StillPending)
                .Where(t => allUserWarehouses.Contains(t.ToWarehouse))
                .ToList();

        }

        public MakeEditTransferViewModel GetReturnBackModel(Transfer transfer)
        {
            MakeEditTransferViewModel model = new MakeEditTransferViewModel();


            model.Transfer = transfer;
            var (activeUser, _) = this.GetActiveUser(this._db);
            model.AllWarehouses = activeUser.GetAllMyWarehouses(this._db, UserFetchOptions.Default);
            model.FromWarehouse = transfer.FromWarehouse;
            model.ToWarehouse = transfer.ToWarehouse;
            model.FromWarehouseId = transfer.FromWarehouseId;
            model.ToWarehouseId = transfer.ToWarehouseId;
            model.AllFromWarehouseItems = transfer.FromWarehouse.GetMovingItemViewModels();
            model.LastChangedToWarehouseId = transfer.ToWarehouseId;

            return model;
        }

        public ActionResult Index()
        {
            var (activeUser, db) = this.GetActiveUser();

            List<Warehouse> allUserWarehouses =
                activeUser.GetAllMyWarehouses(db, UserFetchOptions.IncludeTransfersWithTransferItems);

            List<Transfer> allTransfers = this.GetAllTransfers(allUserWarehouses)
                .OrderByDescending(t=>t.MadeOn)
                .ToList();

            List<Transfer> pendingTransfers = this.GetAllPendingTransfers(allTransfers, allUserWarehouses, activeUser);

            ManageTransfersViewModel model = new ManageTransfersViewModel()
            {
                AllTransfers = allTransfers,
                TransfersToAcceptOrReject = pendingTransfers,
            };

            return View(model);
        }

        public ActionResult DetailsTransferHistory(int transferId)
        {
            Transfer? transfer = _db.Transfers
                .Include(t => t.TransferItems.Select(t => t.Item))
                .Include(transfer => transfer.FromWarehouse)
                .Include(transfer => transfer.ToWarehouse)
                .FirstOrDefault(t => t.Id == transferId);

            if (transfer is null)
            {
                return RedirectToAction("Index", "Transfers");
            }

            DetailsTransferHistoryViewModel model = new DetailsTransferHistoryViewModel()
            {
                Transfer = transfer,
                FromWarehouse = transfer.FromWarehouse,
                ToWarehouse = transfer.ToWarehouse,
            };

            return View(model);
        }

        public ActionResult DetailsPendingTransfer(int transferId)
        {
            Transfer? transfer = this._db.Transfers
                .Include(t => t.TransferItems.Select(t => t.Item))
                .Include(transfer => transfer.FromWarehouse)
                .Include(transfer => transfer.ToWarehouse)
                .FirstOrDefault(t => t.Id == transferId);

            if (transfer is null)
            {
                return RedirectToAction("Index", "Transfers");
            }

            DetailsTransferHistoryViewModel model = new DetailsTransferHistoryViewModel()
            {
                Transfer = transfer,
                FromWarehouse = transfer.FromWarehouse,
                ToWarehouse = transfer.ToWarehouse,
            };

            return View(model);
        }

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

        [HttpPost]
        public ActionResult Make(MakeEditTransferViewModel model)
        {
            var (activeUser, db) = this.GetActiveUser();
            Warehouse fromWarehouse = activeUser.GetWarehouse(model.FromWarehouseId);
            Warehouse toWarehouse = activeUser.GetWarehouse(model.ToWarehouseId);

            List<MovingItem> movingItems = model.AllFromWarehouseItems
                .Where(item => item.IncludeAll || item.TransferAmount != 0)
                .Select(movingItemViewModel =>
                {
                    if (movingItemViewModel.IncludeAll)
                    {
                        movingItemViewModel.TransferAmount = (int) movingItemViewModel.AvailableAmount;
                    }

                    return movingItemViewModel;
                })
                .Select(item => new MovingItem(item))
                .ToList();

            if (movingItems.Count == 0)
            {
                TempData["ErrorMessage"] = ErrorHandler.ErrorMessages.TRANSFER_HAS_NO_ITEMS;
                return RedirectToAction("Make", "Transfers",
                    routeValues: new { fromWarehouseId = model.FromWarehouseId, toWarehouseId = model.ToWarehouseId });
            }

            foreach (MovingItem movingItem in movingItems)
            {
                Item item = fromWarehouse.StoredItems.FirstOrDefault(i => i.Id == movingItem.ItemId);

                if (item is null)
                {
                    this.SetError($"Item with id: {movingItem.ItemId} is missing from warehouse.");
                    return RedirectToAction("Make", "Transfers",
                        routeValues: new { fromWarehouseId = model.FromWarehouseId, toWarehouseId = model.ToWarehouseId });
                }

                if (item.Amount < movingItem.Amount)
                {
                    this.SetError($"Insufficient amounts of the item: {item.Name}.");
                    return RedirectToAction("Make", "Transfers",
                        routeValues: new { fromWarehouseId = model.FromWarehouseId, toWarehouseId = model.ToWarehouseId });
                }

                item.Amount -= movingItem.Amount;
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

        public ActionResult Edit(int transferId)
        {
            var (activeUser, db) = this.GetActiveUser();

            List<Warehouse> allUserWarehouses = activeUser.GetAllMyWarehouses(db, UserFetchOptions.Default);

            Transfer? transfer = this._db.Transfers
                .Include(t => t.TransferItems.Select(t => t.Item))
                .Include(transfer => transfer.FromWarehouse.StoredItems)
                .Include(transfer => transfer.ToWarehouse)
                .FirstOrDefault(t => t.Id == transferId);

            if (transfer is null)
            {
                this.SetError("Transfer is null.");
                return RedirectToAction("Index", "Transfers");
            }


            MakeEditTransferViewModel model = new MakeEditTransferViewModel()
            {
                Transfer = transfer,
                AllWarehouses = allUserWarehouses,
                FromWarehouse = transfer.FromWarehouse,
                ToWarehouse = transfer.ToWarehouse,
                AllFromWarehouseItems = transfer.FromWarehouse.GetMovingItemViewModels(),
                FromWarehouseId = transfer.FromWarehouseId,
                ToWarehouseId = transfer.ToWarehouseId,
                LastChangedToWarehouseId = transfer.ToWarehouseId,
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(MakeEditTransferViewModel model)
        {
            //TODO: The include part can be optimized to be called only when needed
            Transfer transfer = this._db.Transfers
                .Include(transfer => transfer.TransferItems.Select(t => t.Item))
                .Include(transfer => transfer.FromWarehouse.StoredItems)
                .Include(transfer => transfer.ToWarehouse)
                .FirstOrDefault(t => t.Id == model.Transfer.Id);


            if (transfer is null)
            {
                this.SetError("Transfer does not exist anymore.");
                return RedirectToAction("Index", "Transfers");
            }

            bool changeInWarehousePost = false;

            if (model.ToWarehouseId != model.LastChangedToWarehouseId)
            {
                Warehouse changedWarehouse = this._db
                    .Warehouses
                    .First(w => w.Id == model.ToWarehouseId);

                transfer.ToWarehouse = changedWarehouse;
                transfer.ToWarehouseId = changedWarehouse.Id;
                model.LastChangedToWarehouseId = transfer.ToWarehouseId;
                this._db.Entry(transfer).State = EntityState.Modified;
                changeInWarehousePost = true;
            }


            List<MovingItemViewModel> updatedTransferItems = model.AllFromWarehouseItems
                .Where(item => item.IncludeAll || item.TransferAmount != 0)
                .Select(movingItemViewModel =>
                {
                    if (movingItemViewModel.IncludeAll)
                    {
                        movingItemViewModel.TransferAmount += (int) movingItemViewModel.AvailableAmount ;
                    }

                    return movingItemViewModel;
                })
                .ToList();


            List<MovingItem> removeTheseItems = transfer.TransferItems
                .ToList()
                .Where(transferItem =>
                {
                    // ReSharper disable once SimplifyLinqExpressionUseAll
                    bool found = updatedTransferItems
                        .Any(movingItemVm => movingItemVm.Id == transferItem.ItemId);
                    return !found;
                })
                .ToList();

            removeTheseItems.ForEach(item =>
            {
                item.Item.Amount += item.Amount;
                this._db.MovingItems.Remove(item);
                transfer.TransferItems.Remove(item);
            });

            List<string> invalidTransferAmounts = new List<string>();

            foreach (var movingItemVm in updatedTransferItems)
            {
                MovingItem transferItem = transfer
                    .TransferItems
                    .FirstOrDefault(item => item.ItemId == movingItemVm.Id);


                if (transferItem == null)
                {
                    
                    transferItem = new MovingItem
                    {
                        ItemId = movingItemVm.Id,
                        Item = this._db
                            .Items.First(i => i.Id == movingItemVm.Id),
                        Amount = movingItemVm.TransferAmount,
                        EstPrice = movingItemVm.EstPrice,
                    };

                    if (movingItemVm.TransferAmount > movingItemVm.AvailableAmount || movingItemVm.TransferAmount < 0)
                    {
                        invalidTransferAmounts.Add(movingItemVm.Name);
                        movingItemVm.TransferAmount = 0;
                        transferItem.Amount = 0;
                    }
                    else
                    {
                        transferItem.Item.Amount -= movingItemVm.TransferAmount;
                    }
                    transfer.TransferItems.Add(transferItem);
                }
                else
                {
                    

                    int dif = (transferItem.Amount - movingItemVm.TransferAmount);
                    if (-dif > movingItemVm.AvailableAmount || movingItemVm.TransferAmount < 0)
                    {
                        invalidTransferAmounts.Add(movingItemVm.Name);
                         movingItemVm.TransferAmount = transferItem.Amount;

                    }
                    else
                    {
                        transferItem.Item.Amount += dif;
                        transferItem.Amount = movingItemVm.TransferAmount;


                    }
                    transferItem.EstPrice = movingItemVm.EstPrice;
                    

                    this._db.Entry(transferItem).State = EntityState.Modified;
                }
            }


            if (changeInWarehousePost)
            {
                // MVC PLEASE GO AND UNALIVE YOURSELF.....
                ModelState.Remove(nameof(model.LastChangedToWarehouseId));
            }

            if (invalidTransferAmounts.Count > 0)
            {
                model = this.GetReturnBackModel(transfer);
                model.ErrorMessage =
                    $"Items: {string.Join(", ", invalidTransferAmounts)} have invalid transfer amount.";

                // MVC PLEASE GO AND UNALIVE YOURSELF.....

                ModelState.Clear();

                return View(model);
            }

            if (transfer.TransferItems.Count == 0)
            {
                model = this.GetReturnBackModel(transfer);
                model.ErrorMessage = "Transfer edit was not saved, because a transfer must include at least one item.";
                return View(model);
            }

            if (changeInWarehousePost)
            {
                model = this.GetReturnBackModel(transfer);
                return View(model);
            }

            if (model.ToWarehouseId != transfer.ToWarehouseId)
            {
                Warehouse changedWarehouse = this._db
                    .Warehouses
                    .First(w => w.Id == model.ToWarehouseId);

                transfer.ToWarehouse = changedWarehouse;
                transfer.ToWarehouseId = changedWarehouse.Id;
            }

            this
                ._db
                .SaveChanges();


            return RedirectToAction("Index", "Transfers");
        }

        public TryResult TryRejectTransfer(Transfer transfer)
        {
            if (transfer is null)
            {
                return new TryResult(false, new NullReferenceException());
            }

            foreach (MovingItem movingItem in transfer.TransferItems)
            {
                if (movingItem.Item.WarehouseId != null)
                {
                    movingItem.Item.Amount += movingItem.Amount;
                }
                else
                {
                    movingItem.Item.Amount = movingItem.Amount;
                    movingItem.Item.WarehouseId = transfer.FromWarehouseId;
                }
            }

            try
            {
                transfer.Status = TransferStatus.Rejected;
                transfer.ClosedOn = DateTime.Now;

                this._db.Entry(transfer).State = EntityState.Modified;
                this._db.SaveChanges();
            }
            catch (Exception e)
            {
                return new TryResult(false, e);
            }

            return new TryResult(true, null);
        }

        public TryResult TryAcceptTransfer(Transfer transfer)
        {
            if (transfer is null)
            {
                return new TryResult(false, new NullReferenceException($"Transfer is null"));
            }

            try
            {
                List<MovingItem> movingItems = transfer.TransferItems.ToList();

                foreach (MovingItem movingItem in movingItems)
                {
                    Item item = movingItem.Item;
                    


                    Item itemInToWarehouse =
                        transfer.ToWarehouse.StoredItems.FirstOrDefault(i =>
                            i.Name == item.Name && i.Description == item.Description);

                    if (itemInToWarehouse is null)
                    {
                        itemInToWarehouse = new Item
                        {
                            Name = item.Name,
                            Description = item.Description,
                            Type = item.Type,
                            Size = item.Size,
                            EstPrice = item.Size,
                            Amount = movingItem.Amount,
                            ImageUrl = item.ImageUrl,
                            WarehouseId = transfer.ToWarehouseId,
                            Warehouse = transfer.ToWarehouse
                        };

                        this._db.Items.Add(itemInToWarehouse);
                        transfer.ToWarehouse.StoredItems.Add(itemInToWarehouse);
                    }
                    else
                    {
                        itemInToWarehouse.Amount += movingItem.Amount;
                        this._db.Entry(itemInToWarehouse).State = EntityState.Modified;
                    }

                }

                if (!transfer.ToWarehouse.HasEnoughSpace())
                {
                    return new TryResult(false, new Exception($"Warehouse: {transfer.ToWarehouse.Name} does not have enough space for all items in the transfer with id {transfer.Id}."));
                }

                transfer.Status = TransferStatus.Accepted;
                transfer.ClosedOn = DateTime.Now;

                this._db.Entry(transfer).State = EntityState.Modified;
                this._db.SaveChanges();

            }
            catch (Exception e)
            {
                return new TryResult(false, e);
            }

            return new TryResult(true, null);
        }

        public ActionResult Accept(int transferId)
        {
            Transfer? transfer = this._db.Transfers
                .Include(t => t.FromWarehouse.StoredItems)
                .Include(t => t.ToWarehouse.StoredItems)
                .Include(t => t.TransferItems.Select(movingItem => movingItem.Item))
                .FirstOrDefault(t => t.Id == transferId);

            TryResult tryResult = this.TryAcceptTransfer(transfer);

            if (!tryResult.Result)
            {
                this.SetError(tryResult.Exception!.Message);
            }

            return RedirectToAction("Index", "Transfers");
        }

        public ActionResult AcceptAll()
        {
            var (activeUser, _) = this.GetActiveUser(this._db);

            List<Warehouse> allUserWarehouses = activeUser.GetAllMyWarehouses(this._db, UserFetchOptions.Default);

            List<Transfer> allTransfers = this.GetAllTransfers(allUserWarehouses);
            List<Transfer> pendingTransfers = this.GetAllPendingTransfers(allTransfers, allUserWarehouses, activeUser);

            if (pendingTransfers.Count == 0)
            {
                this.SetError("You do not have any transfers to accept.");
                return RedirectToAction("Index", "Transfers");

            }

            int numFailedAccepts = pendingTransfers
                .Select(t => this.TryAcceptTransfer(t).Result)
                .Count(result => !result);

            if (numFailedAccepts == pendingTransfers.Count)
            {
                this.SetError("There was an error while trying to accept all transfers.");
                return RedirectToAction("Index", "Transfers");

            }

            if (numFailedAccepts < pendingTransfers.Count && numFailedAccepts != 0)
            {
                this.SetError("Some transfers failed to be accepted.");
            }

            return RedirectToAction("Index", "Transfers");
        }

        public ActionResult Delete(int transferId)
        {
            Transfer transfer = this._db.Transfers
                .Include(transfer => transfer.TransferItems.Select(movingItem => movingItem.Item))
                .FirstOrDefault(t => t.Id == transferId);

            if (transfer is null)
            {
                return RedirectToAction("Index", "Transfers");
            }

            if (transfer.Status == TransferStatus.StillPending)
            {
                foreach (MovingItem movingItem in transfer.TransferItems)
                {
                    movingItem.Item.Amount += movingItem.Amount;
                }
            }

            
            this._db.Transfers.Remove(transfer);
            this._db.SaveChanges();

            return RedirectToAction("Index", "Transfers");
        }

        public ActionResult DeleteAll()
        {
            var (activeUser, _) = this.GetActiveUser(this._db);

            List<Warehouse> allUserWarehouses = activeUser.GetAllMyWarehouses(this._db, UserFetchOptions.Default);

            List<Transfer> allTransfers = this.GetAllTransfers(allUserWarehouses);

            List<Transfer> transfersToDelete = allTransfers
                .Where(t=>t.MadeByUserId == activeUser.Id && t.Status == TransferStatus.StillPending)
                .ToList();

            if (transfersToDelete.Count == 0)
            {
                this.SetError("You do not have any transfers to delete.");
                return RedirectToAction("Index", "Transfers");
            }

            foreach (Transfer transfer in transfersToDelete)
            {
                foreach (MovingItem movingItem in transfer.TransferItems)
                {
                    if (movingItem.Item.WarehouseId != null)
                    {
                        movingItem.Item.Amount += movingItem.Amount;
                    }
                    else
                    {
                        movingItem.Item.Amount = movingItem.Amount;
                        movingItem.Item.WarehouseId = transfer.FromWarehouseId;
                    }
                }

                this._db.Transfers.Remove(transfer);
            }

            this._db.SaveChanges();

            return RedirectToAction("Index", "Transfers");
        }

        public ActionResult Reject(int transferId)
        {
            Transfer? transfer = this._db.Transfers
                .FirstOrDefault(t => t.Id == transferId);

            TryResult tryResult = this.TryRejectTransfer(transfer);
            if (!tryResult.Result)
            {
                this.SetError($"An error occured while trying to reject the transfer: {tryResult.Exception!.Message}");
            }

            return RedirectToAction("Index", "Transfers");
        }

        public ActionResult RejectAll()
        {
            var (activeUser, _) = this.GetActiveUser(this._db);

            List<Warehouse> allUserWarehouses = activeUser.GetAllMyWarehouses(this._db, UserFetchOptions.Default);

            List<Transfer> allTransfers = this.GetAllTransfers(allUserWarehouses);
            List<Transfer> pendingTransfers = this.GetAllPendingTransfers(allTransfers, allUserWarehouses, activeUser);

            if (pendingTransfers.Count == 0)
            {
                this.SetError("You do not have any transfers to reject.");
                return RedirectToAction("Index", "Transfers");

            }

            int numFailedRejects = pendingTransfers
                .Select(t => this.TryRejectTransfer(t).Result)
                .Count(result => !result);

            if (numFailedRejects == pendingTransfers.Count)
            {
                this.SetError("There was an error while trying to reject all transfers.");
                return RedirectToAction("Index", "Transfers");

            }

            if (numFailedRejects < pendingTransfers.Count && numFailedRejects != 0)
            {
                this.SetError("Rejections of some transfers failed.");
            }

            return RedirectToAction("Index", "Transfers");
        }
    }
}