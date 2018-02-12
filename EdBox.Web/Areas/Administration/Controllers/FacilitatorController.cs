using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EdBox.Web.Controllers;

namespace EdBox.Web.Areas.Administration.Controllers
{
    public class FacilitatorController : BaseController
    {
        // GET: Administration/Facilitator
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FacilitatorConfiguration(string username)
        {
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Index");

            ViewBag.Username = username;
            return View();
        }
    }
}