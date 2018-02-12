using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EdBox.Web.Controllers;

namespace EdBox.Web.Areas.Administration.Controllers
{
    public class UserController : BaseController
    {
        // GET: Administration/User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddUser()
        {
            return View();
        }

        public ActionResult ManageUser(string username)
        {
            using (var data = new Entities())
            {
                ViewBag.Username = username;
                return View(data.Credentials.FirstOrDefault(x => !x.IsDeleted && x.Username == username));
            }
        }

        public ActionResult EditUser(string username)
        {
            using (var data = new Entities())
            {
                ViewBag.Username = username;
                return View(data.Credentials.FirstOrDefault(x => !x.IsDeleted && x.Username == username));
            }
        }
    }
}