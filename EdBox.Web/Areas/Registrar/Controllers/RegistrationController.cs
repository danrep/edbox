using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EdBox.Core;
using EdBox.Web.Controllers;

namespace EdBox.Web.Areas.Registrar.Controllers
{
    public class RegistrationController : BaseController
    {
        // GET: Registrar/Registration
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddStudent()
        {
            return View();
        }

        public ActionResult ManageStudents(string studentId)
        {
            try
            {
                if (string.IsNullOrEmpty(studentId))
                    return RedirectToAction("Index");

                using (var data = new Entities())
                {
                    var student = data.Students.FirstOrDefault(x => x.StudentId == studentId);
                    if (student == null)
                        return RedirectToAction("Index");

                    ViewBag.Class = data.Classes.FirstOrDefault(x => x.Id == student.CurrentClassId)?.ClassName ??
                                    "No Class Assigned";
                    return View(student);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return RedirectToAction("Index");
            }
        }
    }
}