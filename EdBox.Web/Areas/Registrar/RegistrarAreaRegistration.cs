using System.Web.Mvc;

namespace EdBox.Web.Areas.Registrar
{
    public class RegistrarAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Registrar";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Registrar_default",
                "Registrar/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "EdBox.Web.Areas.Registrar.Controllers" }
            );
        }
    }
}