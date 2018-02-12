using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EdBox.Web.Controllers;

namespace EdBox.Web.Areas.Registrar.Controllers
{
    public class AssignmentController : BaseController
    {
        // GET: Registrar/Assignment
        public ActionResult Index()
        {
            return View();
        }
    }
}