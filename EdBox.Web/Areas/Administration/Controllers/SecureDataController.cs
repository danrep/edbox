using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EdBox.Web.Controllers;

namespace EdBox.Web.Areas.Administration.Controllers
{
    public class SecureDataController : BaseController
    {
        public ActionResult PinManagement()
        {
            return View();
        }
    }
}