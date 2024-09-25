using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WarehouseIO.Models;

namespace WarehouseIO.ApiControllers
{
    public class TransfersController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        [ResponseType(typeof(bool))]
        public IHttpActionResult DeleteTransfer(int id)
        {
            Transfer transfer = this._db.Transfers
                .Include(transfer => transfer.TransferItems.Select(movingItem => movingItem.Item))
                .FirstOrDefault(t => t.Id == id);

            if (transfer is null)
            {
                return NotFound();
            }

            if (transfer.Status == TransferStatus.StillPending)
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
                this._db.SaveChanges();
            }
            else
            {
                return InternalServerError(new Exception("Not a pending transfer."));
            }


            return Ok(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}