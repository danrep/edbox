using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EdBox.Core.EnumLib;
using EdBox.Web.Controllers;
using EdBox.Web.Models;

namespace EdBox.Web.Areas.StudentManagement.Controllers
{
    public class PinManagementController : BaseController
    {
        // GET: StudentManagement/PinManagement
        public ActionResult Index(string studentId)
        {
            if (UserInformation.PinRegResultView(studentId) == PinState.NoPinRequired)
                return RedirectToAction("Index", "Home", new { area = "GeneralStaff" });

            using (var data = new Entities())
            {
                var pinBatch = data.PinBatches.FirstOrDefault(x =>
                        x.IsDeleted == false &&
                        x.EducationalPeriod == UserInformation.CurrentEducationalPeriod.Id
                        && x.SubEducationalPeriod == UserInformation.CurrentSubEducationalPeriod.Id);

                if (pinBatch == null)
                    return RedirectToAction("Index", "Home", new { area = "GeneralStaff" });

                var pin = data.PinBatchMembers.FirstOrDefault(x =>
                    x.IsDeleted == false && x.StudentId == studentId && x.BatchId == pinBatch.Id);

                ViewBag.StudentId = studentId;
                return View(pin);
            }
        }
    }
}