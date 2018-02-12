using System;
using System.Linq;
using System.Web.Mvc;
using EdBox.Core;
using EdBox.Web.Controllers;
using EdBox.Web.Models;
using EdBox.Core.EnumLib;
using EdBox.Web.Areas.EducationManagement.Models;

namespace EdBox.Web.Areas.EducationManagement.Controllers
{
    public class ClassManagementController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ClassSubjectManagement(int classSubjectId)
        {
            try
            {
                using (var data = new Entities())
                {
                    var subjectMaps = data.SubjectMaps.Where(x => x.CredentialId == UserInformation.UserInformationCredential.Id).ToList();
                    var classMaps = data.ClassMaps.Where(x => x.CredentialId == UserInformation.UserInformationCredential.Id).ToList();

                    var classSubject =
                        data.ClassSubjects.Where(
                            x =>
                                x.Id == classSubjectId && x.CredentialId == UserInformation.UserInformationCredential.Id)
                            .ToList()
                            .Select(
                                x =>
                                    new ClassSubjectManagement()
                                    {
                                        ClassSubject = x,
                                        SubjectId = subjectMaps.FirstOrDefault(y => y.Id == x.SubjectId)?.SubjectId,
                                        ClassId = classMaps.FirstOrDefault(y => y.Id == x.ClassId)?.ClassId,
                                        ClassName =
                                            data.Classes.ToList().FirstOrDefault(
                                                z => z.Id == classMaps.FirstOrDefault(y => y.Id == x.ClassId)?.ClassId)?
                                                .ClassName ?? "",
                                        SubjectName =
                                            EnumDictionary.GetList<Subjects>().ToList()
                                                .FirstOrDefault(
                                                    z =>
                                                        z.ItemId ==
                                                        subjectMaps.FirstOrDefault(y => y.Id == x.SubjectId)?.SubjectId)
                                                ?
                                                .ItemName ?? ""
                                    }).FirstOrDefault();

                    return classSubject == null ? View("Index") : View(classSubject);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return View("Index");
            }
        }

        public ActionResult ClassOperations(int classId)
        {
            try
            {
                using (var data = new Entities())
                {
                    if (
                        data.ClassMaps.FirstOrDefault(
                            x =>
                                x.IsDeleted == false && x.CredentialId == UserInformation.UserInformationCredential.Id &&
                                x.ClassId == classId) == null)
                        return RedirectToAction("Index");

                    var classInformation = data.Classes.FirstOrDefault(x => x.Id == classId);

                    return View(classInformation);
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return View("Index");
            }
        }
    }
}