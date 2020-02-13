using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UOS.Controllers
{
    public class UserDashboardController : Controller
    {
        //
        // GET: /UserDashboard/
        public ActionResult UDashboard()
        {
            return View();
        }
        public ActionResult Dashboard()
        {
            return View();
        }
	}
}