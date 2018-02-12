using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using EdBox.Web.Areas.Administration.Models;
using EdBox.Web.Controllers;
using EdBox.Core;
using EdBox.Core.EnumLib;

namespace EdBox.Web.ApiControllers.Administration
{
    public class ApiFacilitatorController : BaseController
    {
        [HttpGet]
        public JsonResult MapCredentialSubject(string username, int subjectId, bool state)
        {
            try
            {
                using (var data = new Entities())
                {
                    var user = data.Credentials.FirstOrDefault(x => x.Username == username);

                    if(user == null)
                        return new JsonResult()
                        {
                            Data = new { Status = false, Message = "User does not exist" },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    var map = data.SubjectMaps.FirstOrDefault(x => x.SubjectId == subjectId && x.CredentialId == user.Id);

                    if (map != null)
                    {
                        data.SubjectMaps.Remove(map);
                        data.SaveChanges();
                    }

                    if (state)
                        data.SubjectMaps.Add(new SubjectMap()
                        {
                            CredentialId = user.Id,
                            DateAssigned = DateTime.Now,
                            IsDeleted = false,
                            SubjectId = subjectId
                        });

                    data.SaveChanges();

                    return new JsonResult()
                    {
                        Data = new {Status = true, Message = "Successful"},
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return new JsonResult()
                {
                    Data = new {Status = false, Message = ex.Message, Data = string.Empty},
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        [HttpGet]
        public JsonResult MapCredentialClasses(string username, int classId, bool state)
        {
            try
            {
                using (var data = new Entities())
                {
                    var user = data.Credentials.FirstOrDefault(x => x.Username == username);

                    if (user == null)
                        return new JsonResult()
                        {
                            Data = new { Status = false, Message = "User does not exist" },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    var map = data.ClassMaps.FirstOrDefault(x => x.ClassId == classId && x.CredentialId == user.Id);

                    if (map != null)
                    {
                        data.ClassMaps.Remove(map);
                        data.SaveChanges();
                    }

                    if (state)
                        data.ClassMaps.Add(new ClassMap()
                        {
                            CredentialId = user.Id,
                            DateAssigned = DateTime.Now,
                            IsDeleted = false,
                            ClassId = classId
                        });

                    data.SaveChanges();

                    return new JsonResult()
                    {
                        Data = new { Status = true, Message = "Successful" },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return new JsonResult()
                {
                    Data = new { Status = false, Message = ex.Message, Data = string.Empty },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }
    }
}