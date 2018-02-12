using System.Web.Mvc;

namespace EdBox.Web.Areas.GeneralStaff
{
    public class GeneralStaffAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "GeneralStaff";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "GeneralStaff_default",
                "GeneralStaff/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "EdBox.Web.Areas.GeneralStaff.Controllers" }
            );
        }
    }
}