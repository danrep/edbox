using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EdBox.Core;
using EdBox.Core.EnumLib;
using EdBox.Web.Models;

namespace EdBox.Web.Controllers
{
    public class SecurityController : Controller
    {
        // GET: Security
        public ActionResult ChangePassword()
        {
            if (UserInformation.IsSessionValid)
                return View();
            else
                return RedirectToAction("Index", "Home", new {area = ""});
        }

        public JsonResult ChangePasswordEngine(string oldPassword, string newPassword)
        {
            try
            {
                using (var data = new Entities())
                {
                    if (Encryption.IsSaltEncryptValid(oldPassword,
                        UserInformation.UserInformationCredential.PasswordData,
                        UserInformation.UserInformationCredential.PasswordSalt))
                    {
                        var userData =
                            data.Credentials.FirstOrDefault(
                                x => x.Username == UserInformation.UserInformationCredential.Username);

                        if (userData != null)
                        {
                            userData.PasswordData = Encryption.SaltEncrypt(newPassword, userData.PasswordSalt);
                            userData.UserState = (int)UserStates.Active;

                            data.Entry(userData).State = EntityState.Modified;
                            data.SaveChanges();

                            UserInformation.ActivateSession(userData);
                            return new JsonResult()
                            {
                                Data = new { Status = true, Message = $"Successful" },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet
                            };
                        }
                        else
                            return new JsonResult()
                            {
                                Data = new { Status = false, Message = $"User Credentials are not Valid" },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet
                            };
                    }
                    else
                        return new JsonResult()
                        {
                            Data = new {Status = false, Message = $"Existing Password is Incorrect"},
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