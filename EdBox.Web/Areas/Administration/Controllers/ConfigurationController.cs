using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EdBox.Web.Controllers;

namespace EdBox.Web.Areas.Administration.Controllers
{
    public class ConfigurationController : BaseController
    {
        // GET: Administration/Configuration
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ClassMan()
        {
            return View();
        }

        public ActionResult AddClass()
        {
            return View();
        }

        public ActionResult SessMan()
        {
            return View();
        }

        public ActionResult AssessmentMan()
        {
            return View();
        }

        public ActionResult SubSessMan()
        {
            return View();
        }
    }
}