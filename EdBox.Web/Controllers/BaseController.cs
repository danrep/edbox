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
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (!UserInformation.IsSessionValid)
                {
                    filterContext.Result = RedirectToAction("Index", "Home", new { area = "" });
                    return;
                }

                if (UserInformation.UserInformationCredential.UserState == (int)UserStates.New)
                {
                    if (filterContext.ActionDescriptor.ActionName != "ChangePassword")
                    {
                        filterContext.Result = RedirectToAction("ChangePassword", "Security", new { area = "" });
                        return;
                    }
                }

                var area = filterContext.RouteData.DataTokens["area"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(area))
                {
                    if (area != "EducationManagement" && area != "StudentManagement")
                        if (area != UserInformation.Route)
                        {
                            filterContext.Result = RedirectToAction("Index", "Home", new { area = "" });
                            UserInformation.DeactivateSession();
                            return;
                        }
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
                filterContext.Result = RedirectToAction("Index", "Home", new { area = "" });
                return;
            }
        }
    }
}