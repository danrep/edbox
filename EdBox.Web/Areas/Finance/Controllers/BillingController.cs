using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EdBox.Web.Controllers;

namespace EdBox.Web.Areas.Finance.Controllers
{
    public class BillingController : BaseController
    {
        // GET: Finance/Billing
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult General()
        {
            return View();
        }
        public ActionResult Specific()
        {
            return View();
        }
    }
}