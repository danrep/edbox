using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using EdBox.Core;
using EdBox.Core.EnumLib;
using EdBox.Web.Controllers;
using EdBox.Web.Models;

namespace EdBox.Web.Areas.EducationManagement.Controllers
{
    public class ResultManagementController : BaseController
    {
        public ActionResult Index(int? assessmentId, int? classId, int? subjectId)
        {
            try
            {
                ViewBag.ClassId = classId ?? 0;
                ViewBag.SubjectId = subjectId ?? 0;

                if (assessmentId == null)
                    return View();
                else
                {
                    using (var data = new Entities())
                    {
                        var assessment = data.Assessments.FirstOrDefault(x => x.Id == assessmentId);

                        if (
                            data.ClassMaps.FirstOrDefault(
                                x =>
                                    x.CredentialId == UserInformation.UserInformationCredential.Id &&
                                    x.ClassId == classId) != null &&
                            data.SubjectMaps.FirstOrDefault(
                                x =>
                                    x.CredentialId == UserInformation.UserInformationCredential.Id &&
                                    x.SubjectId == subjectId) != null
                            )
                            return View(assessment);
                        else
                            return View();
                    }
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return View();
            }
        }

        public void ExportDataSheet(int assessmentId, int classId, int subjectId)
        {
            try
            {
                using (var data = new Entities())
                {
                    var assessment = data.Assessments.FirstOrDefault(x => x.Id == assessmentId);
                    if (assessment == null)
                        return;
                    
                    var students = data.Students.Where(x => x.CurrentClassId == classId).ToList();
                    var classInfo = data.Classes.FirstOrDefault(x => x.Id == classId);
                    var subjectInfo = ((Subjects) subjectId).DisplayName();

                    Response.ClearContent();
                    Response.AddHeader("content-disposition",
                        $"attachment;filename={classInfo?.ClassName}_{subjectInfo}_{assessment.AssessmentName}.csv");
                    Response.ContentType = "application/octet-stream";

                    var sw = new StringWriter();
                    sw.WriteLine("\"Student Id\",\"Score Obtained\",\"Total Score\",\"Full Name\"");

                    foreach (var student in students)
                    {
                        sw.WriteLine(
                            $"\"{student.StudentId}\",\"{string.Empty}\",\"{string.Empty}\",\"{$"{student.StudentSurname}, {student.StudentOtherName}"}\"");
                    }

                    Response.Write(sw.ToString());
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
            }
        }

        public ActionResult DisplayUploadedData(int assessmentId = 0, int classId = 0, int subjectId = 0)
        {
            try
            {
                ViewBag.ClassId = classId;
                ViewBag.SubjectId = subjectId;

                if (assessmentId == 0)
                    return RedirectToAction("Index");

                return View(assessmentId);
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return RedirectToAction("Index");
            }
        }

        public ActionResult BulkPrint(int classId)
        {
            using (var data = new Entities())
            {
                return View(classId);
            }
        }
    }
}