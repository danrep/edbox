using System.Web.Mvc;

namespace EdBox.Web.Areas.EducationManagement
{
    public class EducationManagementAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "EducationManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "EducationManagement_default",
                "EducationManagement/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "EdBox.Web.Areas.EducationManagement.Controllers" }
            );
        }
    }
}