using System.Web.Mvc;
using YASop.AdminUI.Code;

namespace DiscontMD.WebUI.Controllers
{
    public class DiscontController : Controller
    {
        protected virtual bool RequireLogin => false;
        protected bool IsLoggedIn => new AuthHelper().IsAuthenticated;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (RequireLogin && !IsLoggedIn) throw new RequireLoginException();
            base.OnActionExecuting(filterContext);
        }
    }
}