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
    public class ItemsController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // DELETE: api/Items/5
        [ResponseType(typeof(bool))]
        public IHttpActionResult DeleteItem(int id)
        {
            Item item = this._db
                .Items
                .FirstOrDefault(i => i.Id == id);

            if (item is null)
            {
                return NotFound();
            }

            item.Warehouse = null;
            item.WarehouseId = null;

            this._db.Entry(item).State = EntityState.Modified;
            this._db.SaveChanges();

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