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
    public class ShipmentsController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        
        [ResponseType(typeof(bool))]
        public IHttpActionResult DeleteShipment(int id)
        {
            Shipment shipment = this
                ._db
                .Shipments.Include(shipment => shipment.ShippingItems.Select(movingItem => movingItem.Item))
                .Include(shipment => shipment.FromWarehouse)
                .FirstOrDefault(s => s.Id == id);

            if (shipment is null)
            {
                return NotFound();
            }

            TryResult tryResult = shipment.TryCancel(this._db);

            if (!tryResult.Result)
            {
                return InternalServerError(tryResult.Exception);
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