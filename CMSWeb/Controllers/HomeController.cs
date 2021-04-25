using CMSWeb.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMSWeb.Controllers
{
    public class HomeController : Controller
    {
        //SessionContext context = new SessionContext();

        public ActionResult Index()
        {
            if (!SessionContext.IsAuthentication().Item1)
                return RedirectToAction("Index", "Login");

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}