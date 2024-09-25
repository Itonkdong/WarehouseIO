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
    public class WarehousesController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        
        // DELETE: api/Warehouses/5
        [ResponseType(typeof(void))]
        public IHttpActionResult DeleteWarehouse(int id)
        {
            try
            {
                Warehouse warehouse = this._db
                    .Warehouses
                    .Include(w => w.TransfersFromWarehouse)
                    .Include(w => w.Operators)
                    .Include(w => w.Managers)
                    .Include(w => w.TransfersToWarehouse
                        .Select(t => t.TransferItems
                            .Select(mi => mi.Item)))
                    .FirstOrDefault(w => w.Id == id);
                if (warehouse is null)
                {
                    return NotFound();
                }

                List<Transfer> transfers = warehouse.TransfersFromWarehouse.ToList();

                foreach (Transfer transfer in transfers)
                {
                    this._db
                        .Transfers
                        .Remove(transfer);
                }


                 foreach (Transfer transfer in warehouse.TransfersToWarehouse)
                 {
                     transfer.TryReject(this._db);
                     transfer.Status = TransferStatus.WarehouseDoesNotExistAnymore;
                 }


                 warehouse.Operators.Clear();
                 warehouse.Managers.Clear();

                this._db.SaveChanges();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
            

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}