using System.Web.Mvc;
using DiscontMD.BusinessLogic;
using YASop.AdminUI.Code;

namespace DiscontMD.WebUI.Controllers
{
    public class DiscontController : Controller
    {
        protected virtual bool RequireLogin => false;
        protected bool IsLoggedIn =>  Registry.Current.Services.User.IsAuthenticated;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (RequireLogin && !IsLoggedIn) throw new RequireLoginException();
            base.OnActionExecuting(filterContext);
        }
    }
}