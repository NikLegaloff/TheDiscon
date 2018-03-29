using System.Dynamic;
using System.Threading.Tasks;
using System.Web.Mvc;
using DiscontMD.BusinessLogic;
using DiscontMD.BusinessLogic.DomainModel;
using YASop.AdminUI.Code;

namespace DiscontMD.WebUI.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LogOff()
        {
            new AuthHelper().LogOff();
            return Redirect("/");
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Login()
        {
            dynamic res = new ExpandoObject();
            res.Error = "";
            return View(res);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Login(string email, string pass)
        {
            var user = await Registry.Current.Services.User.Get(email, pass);
            if (user != null)
            {
                new AuthHelper().Authenticate(user.Id);
                return Redirect("/");
            }
            dynamic res = new ExpandoObject();
            res.Error = "Invalid email/password";
            return View(res);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Registration()
        {
            dynamic res = new ExpandoObject();
            res.Error = "";
            return View(res);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Registration(string email, string pass, string pass2)
        {

            dynamic res = new ExpandoObject();
            res.Error = "";
            var user = await Registry.Current.Services.User.Get(email);
            if (pass != pass2)
            {
                res.Error = "Пароли не совпадают";
                return View(res);
            }
            if (user != null)
            {
                res.Error = email + " уже зарегистрирован в системе";
                return View(res);
            }
            await Registry.Current.Services.User.Register(email, pass, UserRole.Admin);
            return await Login(email, pass);
        }
    }
}