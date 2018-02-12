using System.Web.Mvc;

namespace EdBox.Web.Areas.StudentManagement
{
    public class StudentManagementAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "StudentManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "StudentManagement_default",
                "StudentManagement/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "EdBox.Web.Areas.StudentManagement.Controllers" }
            );
        }
    }
}