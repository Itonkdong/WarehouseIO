using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseIO.Controllers
{
    [Authorize]
    public class ShipmentsController : Controller
    {
        // GET: Shipment
        public ActionResult Index()
        {
            return View();
        }

        // GET: Shipment/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Shipment/Create
        public ActionResult Make()
        {
            return View();
        }

        // POST: Shipment/Create
        [HttpPost]
        public ActionResult Make(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        /*// GET: Shipment/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Shipment/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
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

        // GET: Shipment/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Shipment/Delete/5
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
