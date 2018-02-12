using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EdBox.Core;
using EdBox.Web.Controllers;
using EdBox.Web.Models;

namespace EdBox.Web.ApiControllers.StudentManagement
{
    public class ApiStudentManagementController : BaseController
    {
        [HttpGet]
        public JsonResult ProcessPin(string pin, string studentId)
        {
            try
            {
                using (var data = new Entities())
                {
                    var student = data.Students.FirstOrDefault(x => x.IsDeleted == false && x.StudentId == studentId);

                    if(student == null)
                        return new JsonResult()
                        {
                            Data =
                                new
                                {
                                    Status = false,
                                    Message = $"This Student Id is not recognized",
                                    Data = string.Empty
                                },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    var pinData = data.PinBatchMembers.FirstOrDefault(x => x.IsDeleted == false && x.PinData == pin);

                    if (pinData == null)
                        return new JsonResult()
                        {
                            Data =
                                new
                                {
                                    Status = false,
                                    Message = $"This PIN is not recognised on this Platform",
                                    Data = string.Empty
                                },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    if (pinData.IsUsed)
                        return new JsonResult()
                        {
                            Data =
                                new
                                {
                                    Status = false,
                                    Message = $"This PIN has been Used",
                                    Data = string.Empty
                                },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    var batch = data.PinBatches.FirstOrDefault(x => x.Id == pinData.BatchId);

                    if (batch.EducationalPeriod != UserInformation.CurrentEducationalPeriod.Id && batch.SubEducationalPeriod != UserInformation.CurrentSubEducationalPeriod.Id)
                        return new JsonResult()
                        {
                            Data =
                                new
                                {
                                    Status = false,
                                    Message = $"This PIN is Expired",
                                    Data = string.Empty
                                },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    pinData.DateUsed = DateTime.Now;
                    pinData.IsUsed = true;
                    pinData.CredentialId = UserInformation.UserInformationCredential.Id;
                    pinData.StudentId = studentId;

                    data.Entry(pinData).State = EntityState.Modified;
                    data.SaveChanges();

                    return new JsonResult()
                    {
                        Data =
                            new
                            {
                                Status = true,
                                Message = $"All Done",
                                Data = string.Empty
                            },
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