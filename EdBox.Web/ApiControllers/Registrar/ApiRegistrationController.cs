using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using EdBox.Core;
using EdBox.Core.EnumLib;
using EdBox.Web.Areas.Registrar.Models;
using EdBox.Web.Controllers;
using EdBox.Web.Models;

namespace EdBox.Web.ApiControllers.Registrar
{
    public class ApiRegistrationController : BaseController
    {
        [HttpPost]
        public JsonResult Registration(StudentInformation studentInformation)
        {
            try
            {
                //studentInformation.StudentId = DateTime.Now.ToString("yyyyMMddhhmmss");

                if (string.IsNullOrEmpty(studentInformation.StudentImage))
                    return new JsonResult()
                    {
                        Data = new { Status = false, Message = "Please ensure that you have uploaded an Image", Data = string.Empty },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };

                using (var data = new Entities())
                {
                    if (data.Students.FirstOrDefault(
                            x => x.StudentId == studentInformation.StudentId && x.IsDeleted == false) != null)
                        return new JsonResult()
                        {
                            Data =
                                new
                                {
                                    Status = false,
                                    Message =
                                        "This Student Id is in Use. Please confirm",
                                    Data = string.Empty
                                },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    if (
                        data.Students.FirstOrDefault(
                            x =>
                                x.PGEmail == studentInformation.PgEmail &&
                                x.StudentSurname == studentInformation.StudentSurname &&
                                x.StudentOtherName == studentInformation.StudentOtherName) != null)
                        return new JsonResult()
                        {
                            Data =
                                new
                                {
                                    Status = false,
                                    Message =
                                        "It would seem that you have added this student before now. Please confirm",
                                    Data = string.Empty
                                },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    var authenticationInfo =
                        data.Credentials.FirstOrDefault(x => x.Username == studentInformation.PgEmail);

                    if (authenticationInfo == null)
                    {
                        var salt = Encryption.GetUniqueKey(6);
                        authenticationInfo = data.Credentials.Add(new Credential()
                        {
                            DateRegistered = DateTime.Now,
                            FirstName = studentInformation.PgFullName,
                            IsDeleted = false,
                            LastName = "",
                            PasswordData = Encryption.SaltEncrypt(Encryption.GetUniqueKey(6), salt),
                            PasswordSalt = salt,
                            UserState = (int)UserStates.New,
                            Username = studentInformation.PgEmail
                        });

                        data.CredentialMaps.Add(new CredentialMap()
                        {
                            CredentialId = authenticationInfo.Id,
                            DateAssigned = DateTime.Now,
                            IsDeleted = false,
                            RoleId = (int)UserRoles.Students
                        });

                        data.SaveChanges();
                        Messenger.SendNewCredentials(studentInformation.PgEmail);
                    }

                    var location = $"{HostingEnvironment.ApplicationPhysicalPath}UploadedFile";
                    var path = $"{location}\\{studentInformation.StudentImage}";
                    data.Students.Add(new Student()
                    {
                        IsDeleted = false,
                        AuuthCredentialId = authenticationInfo.Id,
                        BriefInformation = studentInformation.BriefInformation,
                        CurrentClassId = 0,
                        PGEmail = studentInformation.PgEmail,
                        PGFullName = studentInformation.PgFullName,
                        PGPhone = studentInformation.PgPhone,
                        StudentImage = Convert.ToBase64String(System.IO.File.ReadAllBytes(path)),
                        StudentHomeAddress = studentInformation.StudentHomeAddress,
                        StudentId = studentInformation.StudentId,
                        StudentOtherName = studentInformation.StudentOtherName,
                        StudentSurname = studentInformation.StudentSurname
                    });

                    data.SaveChanges();

                    Messenger.SendStudentEntry(studentInformation.PgEmail, studentInformation.StudentId);
                    System.IO.File.Delete(path);
                    return new JsonResult()
                    {
                        Data = new { Status = true, Message = $"Successful" },
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

        [HttpPost]
        public JsonResult UpdateRegistration(StudentInformation studentInformation)
        {
            try
            {
                using (var data = new Entities())
                {
                    var existingData = data.Students.FirstOrDefault(x => x.StudentId == studentInformation.StudentId);

                    if (existingData == null)
                        return new JsonResult()
                        {
                            Data =
                                new
                                {
                                    Status = false,
                                    Message =
                                        "This User does NOT exist. Please try again",
                                    Data = string.Empty
                                },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    if (
                        data.Students.FirstOrDefault(
                            x =>
                                x.PGEmail == existingData.PGEmail &&
                                x.StudentSurname == studentInformation.StudentSurname &&
                                x.StudentOtherName == studentInformation.StudentOtherName &&
                                x.StudentId != studentInformation.StudentId) != null)
                        return new JsonResult()
                        {
                            Data =
                                new
                                {
                                    Status = false,
                                    Message =
                                        "It would seem that you have added this student before now. Please confirm",
                                    Data = string.Empty
                                },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    existingData.BriefInformation = studentInformation.BriefInformation;
                    existingData.CurrentClassId = studentInformation.CurrentClassId;
                    existingData.StudentHomeAddress = studentInformation.StudentHomeAddress;
                    existingData.StudentOtherName = studentInformation.StudentOtherName;
                    existingData.StudentSurname = studentInformation.StudentSurname;
                    
                    data.Entry(existingData).State = EntityState.Modified;
                    data.SaveChanges();

                    return new JsonResult()
                    {
                        Data = new { Status = true, Message = $"Successful" },
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

        [HttpGet]
        public JsonResult GetRegistrants(string queryString)
        {
            try
            {
                using (var data = new Entities())
                {
                    var classes = data.Classes.Where(x => x.IsDeleted == false).ToList();
                    var students = (from s in data.Students.Where(x => x.IsDeleted == false)
                                    where
                                        s.PGEmail.Contains(queryString) ||
                                        s.StudentOtherName.Contains(queryString) ||
                                        s.StudentSurname.Contains(queryString) ||
                                        s.BriefInformation.Contains(queryString) ||
                                        s.PGFullName.Contains(queryString) ||
                                        s.PGEmail.Contains(queryString) ||
                                        s.PGPhone.Contains(queryString) ||
                                        s.StudentId.Contains(queryString)
                                    select s).Take(20)
                        .OrderBy(x => x.StudentId)
                        .ThenBy(x => x.StudentSurname)
                        .ThenBy(x => x.StudentOtherName)
                        .ToList();

                    return new JsonResult()
                    {
                        Data =
                            new
                            {
                                Status = true,
                                Message = $"All Done",
                                Data =
                                    students.Select(
                                        s =>
                                            new
                                            {
                                                s,
                                                CurrentClass =
                                                    classes.FirstOrDefault(x => x.Id == s.CurrentClassId)?.ClassName ??
                                                    "No Class Assigned"
                                            })
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

        [HttpGet]
        public JsonResult DeleteRegistration(string studentId)
        {
            try
            {
                using (var data = new Entities())
                {
                    var student = data.Students.FirstOrDefault(x => x.StudentId == studentId && x.IsDeleted == false);

                    if (student == null)
                        return new JsonResult()
                        {
                            Data =
                                new
                                {
                                    Status = false,
                                    Message = $"This Student is not found. Please Check",
                                    Data = ""
                                },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };

                    student.IsDeleted = true;
                    data.Entry(student).State = EntityState.Modified;
                    data.SaveChanges();

                    return new JsonResult()
                    {
                        Data =
                            new
                            {
                                Status = true,
                                Message = $"Successful",
                                Data = ""
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

        [HttpGet]
        public JsonResult ManageSubjects(int subjectId, string studentId, bool newValue)
        {
            try
            {
                using (var data = new Entities())
                {
                    var studentSubject =
                        data.StudentSubjectMaps.FirstOrDefault(x => x.StudentId == studentId && x.SubjectId == subjectId);

                    if (studentSubject == null)
                    {
                        data.StudentSubjectMaps.Add(new StudentSubjectMap()
                        {
                            DateAssigned = DateTime.Now,
                            IsDeleted = false,
                            StudentId = studentId,
                            SubjectId = subjectId
                        });
                    }
                    else
                    {
                        studentSubject.IsDeleted = !newValue;
                        studentSubject.DateAssigned = DateTime.Now;
                        data.Entry(studentSubject).State = EntityState.Modified;
                    }

                    data.SaveChanges();
                }

                return new JsonResult()
                {
                    Data =
                        new
                        {
                            Status = true,
                            Message = $"All Done",
                            Data = ""
                        },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return new JsonResult()
                {
                    Data = new { Status = false, Message = ex.Message + ". Please Refresh the page.", Data = string.Empty },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }
    }
}