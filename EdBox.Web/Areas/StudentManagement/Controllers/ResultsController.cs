using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using EdBox.Core;
using EdBox.Core.EnumLib;
using EdBox.Web.Controllers;
using EdBox.Web.Models;

namespace EdBox.Web.Areas.StudentManagement.Controllers
{
    public class ResultsController : Controller
    {
        // GET: StudentManagement/Results
        public ActionResult Index(string studentId)
        {
            try
            {
                if (!UserInformation.IsSessionValid)
                    return RedirectToAction("Index", "Home", new { area = "GeneralStaff" });

                if (string.IsNullOrEmpty(studentId))
                    return RedirectToAction("Index", "Home", new { area = "GeneralStaff" });

                using (var data = new Entities())
                {
                    var studentData = data.Students.FirstOrDefault(x => x.StudentId == studentId);
                    if (studentData == null)
                        return RedirectToAction("Index", "Home", new { area = "GeneralStaff" });

                    var pinState = UserInformation.PinRegResultView(studentId);

                    switch (pinState)
                    {
                        case PinState.NoPinRequired:
                            return View(studentData);
                        case PinState.PinRequiredFound:
                            return View(studentData);
                        case PinState.PinRequiredNotFound:
                            return RedirectToAction("Index", "PinManagement", new {area = "StudentManagement"});
                        default:
                            return RedirectToAction("Index", "Home", new { area = "GeneralStaff" });
                    }
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return RedirectToAction("Index", "Home", new {area = "GeneralStaff"});
            }
        }

        public ActionResult PrintResult(string studentId = "", bool isSession = false, bool isThreaded = false, string key = "", int classId = 0)
        {
            try
            {
                if (!string.IsNullOrEmpty(key))
                {
                    if (!UserInformation.IsSessionValid)
                        return RedirectToAction("Index", "Home", new { area = "GeneralStaff" });

                    if (UserInformation.UserInformationCredential.Username != Encryption.SaltDecrypt(key, DateTime.Now.ToString("YYYYMMDD")))
                        return RedirectToAction("Index", "Home", new { area = "GeneralStaff" });

                    if (isSession)
                        return RedirectToAction("Index");

                    Session["IsSession"] = isSession;

                    var listOfStudents = new List<Student>();

                    using (var data = new Entities())
                    {
                        if (!string.IsNullOrEmpty(studentId))
                        {
                            listOfStudents.Add(data.Students.FirstOrDefault(x => x.StudentId == studentId));
                            ViewBag.ResultName = "Result for " + studentId;
                        }
                        else
                        {
                            listOfStudents.AddRange(data.Students.Where(x => x.CurrentClassId == classId).ToList());
                            ViewBag.ResultName = "Result for " + data.Classes.FirstOrDefault(x => x.Id == classId)?.ClassName ?? "No Class";
                        }
                    }

                    return View(listOfStudents);
                }
                else
                {
                    return RedirectToAction("Index", "Home", new { area = "GeneralStaff" });
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