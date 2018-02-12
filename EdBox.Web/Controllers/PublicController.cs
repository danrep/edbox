using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EdBox.Web.Controllers
{
    public class PublicController : Controller
    {
        // GET: Public
        public ActionResult MailTemplate()
        {
            return View();
        }
    }
}