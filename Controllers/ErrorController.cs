using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TAT001.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Error(string aspxerrorpath)
        {
            return View("NotFound");
        }
        // GET: Error
        public ActionResult NotFound(string aspxerrorpath)
        {
            return View("NotFound");
        }
    }
}