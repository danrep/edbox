using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using EdBox.Web.Controllers;
using EdBox.Core;
using EdBox.Core.EnumLib;
using EdBox.Web.Models;
using UserInformation = EdBox.Web.Areas.Administration.Models.UserInformation;

namespace EdBox.Web.ApiControllers.Administration
{
    public class ApiUserController : BaseController
    {
        [HttpGet]
        public JsonResult GetUsers()
        {
            try
            {
                using (var data = new Entities())
                {
                    var registeredUsersByRole =
                        data.CredentialMaps.Where(x => x.IsDeleted == false).ToList();

                    var usersRawPrimary = data.Credentials.Where(x => x.IsDeleted == false).ToList();

                    var usersRaw =
                        usersRawPrimary.Where(x => registeredUsersByRole.Select(y => y.CredentialId).Contains(x.Id))
                            .ToList();

                    var users = usersRaw.Select(
                                s =>
                                    new
                                    {
                                        UserInfo = s,
                                        UserRole =
                                            ((UserRoles)
                                                registeredUsersByRole.FirstOrDefault(r => r.CredentialId == s.Id).RoleId)
                                                .DisplayName()
                                    })
                            .OrderBy(o => o.UserInfo.Username).ThenBy(o => o.UserInfo.LastName)
                            .ToList();

                    return new JsonResult()
                    {
                        Data = new { Status = true, Message = $"Found {users.Count} User(s)", Data = users },
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
        public JsonResult GetUser(string username)
        {
            try
            {
                using (var data = new Entities())
                {
                    var allClasses = data.Classes.Where(x => x.IsDeleted == false).ToList();
                    var allSubjects = EnumDictionary.GetList<Subjects>();

                    var userInformation =
                        data.Credentials.FirstOrDefault(x => x.IsDeleted == false && x.Username == username);

                    var mappedClassesRaw =
                        data.ClassMaps
                        .Where(x => x.IsDeleted == false && x.CredentialId == userInformation.Id).ToList();

                    var mappedSubjectsRaw =
                        data.SubjectMaps.Where(x => x.IsDeleted == false && x.CredentialId == userInformation.Id)
                            .ToList();

                    var mappedClasses =
                        allClasses.Select(
                            c =>
                                new
                                {
                                    c,
                                    MappingStatus = (mappedClassesRaw.FirstOrDefault(m => m.ClassId == c.Id) != null)
                                })
                            .OrderBy(x => x.c.ClassName)
                            .ToList();

                    var mappedSubjects =
                        allSubjects.Select(
                            s => new
                            {
                                s,
                                MappingStatus = (mappedSubjectsRaw.FirstOrDefault(m => m.SubjectId == s.ItemId) != null)
                            })
                            .OrderBy(x => x.s.ItemName)
                            .ToList();

                    return new JsonResult()
                    {
                        Data =
                            new
                            {
                                Status = true,
                                Message = $"Found User",
                                Data =
                                    new
                                    {
                                        UserInformation = userInformation,
                                        MappedClasses = mappedClasses,
                                        MappedSubject = mappedSubjects,
                                        TotalClasses = mappedClasses.Count,
                                        TotalSubjects = mappedSubjects.Count
                                    }
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

        [HttpPost]
        public JsonResult CreateUser(UserInformation userInformation)
        {
            try
            {
                using (var data = new Entities())
                {
                    if (data.Credentials.FirstOrDefault(x => x.Username == userInformation.Username.ToLower()) != null)
                        return new JsonResult() { Data = new { Status = false, Message = "Username has been Used" } };

                    var passwordSalt = Encryption.GetUniqueKey(10);
                    var credential = data.Credentials.Add(new Credential()
                    {
                        DateRegistered = DateTime.Now,
                        FirstName = userInformation.Firstname,
                        IsDeleted = false,
                        LastName = userInformation.Lastname,
                        PasswordData = Encryption.SaltEncrypt(Encryption.GetUniqueKey(6), passwordSalt),
                        PasswordSalt = passwordSalt,
                        UserState = (int)UserStates.New,
                        Username = userInformation.Username.ToLower()
                    });
                    data.SaveChanges();

                    data.CredentialMaps.Add(new CredentialMap()
                    {
                        IsDeleted = false,
                        CredentialId = credential.Id,
                        DateAssigned = DateTime.Now,
                        RoleId = userInformation.UserRole
                    });
                    data.SaveChanges();

                    new Thread(() =>
                    {
                        if (Messenger.SendNewCredentials(userInformation.Username))
                            ActivityLogger.Log("INFO", $"New Credentials have been sent Successfully");
                    }).Start();

                    ActivityLogger.Log("INFO", $"{userInformation.Username}: {userInformation.Lastname}, {userInformation.Firstname} has been created");
                }

                return new JsonResult() { Data = new { Status = true, Message = "Successful" } };
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return new JsonResult() { Data = new { Status = false, Message = ex.Message } };
            }
        }

        [HttpPost]
        public JsonResult UpdateUser(UserInformation userInformation)
        {
            try
            {
                using (var data = new Entities())
                {
                    var existingCredo =
                        data.Credentials.FirstOrDefault(x => x.Username == userInformation.Username.ToLower());

                    if (existingCredo == null)
                        return new JsonResult() { Data = new { Status = false, Message = "This Credential is not Recognised. Please try to Add the User" } };

                    existingCredo.FirstName = userInformation.Firstname;
                    existingCredo.LastName = userInformation.Lastname;
                   
                    data.Entry(existingCredo).State = EntityState.Modified;
                    data.SaveChanges();

                    var existingCredoMap = data.CredentialMaps.FirstOrDefault(x => !x.IsDeleted && x.CredentialId == existingCredo.Id);
                    if (existingCredoMap != null)
                    {
                        data.CredentialMaps.Remove(existingCredoMap);
                        data.SaveChanges();
                    }

                    data.CredentialMaps.Add(new CredentialMap()
                    {
                        IsDeleted = false,
                        CredentialId = existingCredo.Id,
                        DateAssigned = DateTime.Now,
                        RoleId = userInformation.UserRole
                    });
                    data.SaveChanges();

                    ActivityLogger.Log("INFO", $"{userInformation.Username}: {userInformation.Lastname}, {userInformation.Firstname} has been Updated");
                }

                return new JsonResult() { Data = new { Status = true, Message = "Successful" } };
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return new JsonResult() { Data = new { Status = false, Message = ex.Message } };
            }
        }

        [HttpGet]
        public JsonResult DeleteUser(string username)
        {
            try
            {
                using (var data = new Entities())
                {
                    var user = data.Credentials.FirstOrDefault(x => x.Username == username.ToLower() && x.IsDeleted == false);
                    if (user == null)
                        return new JsonResult()
                        {
                            Data = new { Status = false, Message = "Username has NOT been Configured" }
                        };

                    data.CredentialMaps.Remove(data.CredentialMaps.FirstOrDefault(x => x.CredentialId == user.Id));

                    user.IsDeleted = true;

                    data.Entry(user).State = EntityState.Modified;
                    data.SaveChanges();

                    ActivityLogger.Log("INFO", $"{username} has been deleted");
                }

                return new JsonResult() { Data = new { Status = true, Message = "Successful" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return new JsonResult() { Data = new { Status = false, Message = ex.Message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpGet]
        public JsonResult SendUserCredentials(string username)
        {
            try
            {
                using (var data = new Entities())
                {
                    var user = data.Credentials.FirstOrDefault(x => x.Username == username.ToLower() && x.IsDeleted == false);
                    if (user == null)
                        return new JsonResult()
                        {
                            Data = new { Status = false, Message = "Username has NOT been Configured" }
                        };

                    new Thread(() =>
                    {
                        if (Messenger.SendCredentials(username))
                            ActivityLogger.Log("INFO", $"Credentials have been sent Successfully");
                    }).Start();

                    ActivityLogger.Log("INFO", $"{username} has been deleted");
                }

                return new JsonResult() { Data = new { Status = true, Message = "Successful" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                return new JsonResult() { Data = new { Status = false, Message = ex.Message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
    }
}