using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseIO.Controllers
{
    [Authorize]
    public class OperatorsController : Controller
    {
        // GET: Operators
        public ActionResult Index()
        {
            return View();
        }

       
    }
}
