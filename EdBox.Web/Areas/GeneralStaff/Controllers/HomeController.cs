using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EdBox.Web.Controllers;

namespace EdBox.Web.Areas.GeneralStaff.Controllers
{
    public class HomeController : BaseController
    {
        // GET: GeneralStaff/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}