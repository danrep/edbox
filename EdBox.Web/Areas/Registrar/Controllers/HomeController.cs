using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EdBox.Web.Controllers;

namespace EdBox.Web.Areas.Registrar.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Registrar/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}