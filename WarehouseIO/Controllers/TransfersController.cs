﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseIO.Controllers
{
    [Authorize]
    public class TransfersController : Controller
    {
        // GET: Transfer
        public ActionResult Index()
        {
            return View();
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
        public ActionResult Make()
        {
            return View();
        }

        // POST: Transfer/Create
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

        // GET: Transfer/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Transfer/Edit/5
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
