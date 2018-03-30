using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Dapper;
using DiscontMD.BusinessLogic;
using DiscontMD.BusinessLogic.DomainModel;
using DiscontMD.BusinessLogic.Presistense.MSSQL;
using DiscontMD.WebUI.Models;

namespace DiscontMD.WebUI.Controllers
{
    public class AdminController : DiscontController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (RequireLogin && !IsLoggedIn) throw new Exception("Access denied");
            if (Registry.Current.Services.User.CurrentUser().Role!=UserRole.Superuser) throw new Exception("Access denied");
            base.OnActionExecuting(filterContext);
        }

        public ActionResult Index()
        {
            var storeId = Registry.Current.Services.User.CurrentUser().StoreId;
            var dash = new BackofficeDash();
            using (var connection = MSSqlDb.Open())
            {
                dash.ActiveCards = (int)connection.ExecuteScalar("select COUNT(*) from Card where StoreId=@storeId", new { storeId = storeId.Value });
                dash.AvailableCards = (int)connection.ExecuteScalar("select COUNT(*) from AvailableCard where StoreId=@storeId", new { storeId = storeId.Value });
            }

            return View(dash);
        }



        public async Task<ActionResult> SelectStore()
        {
            Store[] stores = await Registry.Current.Data.Stores.Select();
            return View(stores);
        }

    }
}