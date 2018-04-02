using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Dapper;
using DiscontMD.BusinessLogic;
using DiscontMD.BusinessLogic.Bus.Commands;
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



        [AcceptVerbs(HttpVerbs.Get)]
        public async Task<ActionResult> SelectStore()
        {
            Store[] stores = await Registry.Current.Data.Stores.Select();
            return View(stores);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> SelectStore(Guid storeId)
        {
            var currentUser = Registry.Current.Services.User.CurrentUser();
            currentUser.StoreId=storeId;
            await Registry.Current.Data.Users.Save(currentUser);
            return RedirectToAction("Index");
        }

        public ActionResult AddCards(int count)
        {
            var storeId = Registry.Current.Services.User.CurrentUser().StoreId;
            if (storeId != null) Registry.Current.Services.Card.AssignPackToStore(count, storeId.Value);
            else throw new Exception("No current store set");
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public async Task<ActionResult> Print()
        {
            using (var connection = MSSqlDb.Open())
            {
                var result = await connection.QueryAsync("select cp.Id, cp.NumBase, Domain, Name, cp.Prepared from CardPack cp join Store s on cp.StoreId=s.Id where cp.StoreId is not null and cp.Printed=0 order by Domain, NumBase");
                return View(result);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public async Task<ActionResult> DoAction()
        {
            return RedirectToAction("Print");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public async Task<ActionResult> DoPrint(Guid id)
        {
            AsyncHelpers.RunSync(() => PrintMe(id));
            return RedirectToAction("Print");
        }

        private static async Task PrintMe(Guid id)
        {
            var printPagesPackCommandHandler = new PrintPagesPackCommandHandler(Guid.Empty);
            await printPagesPackCommandHandler.Process(new PrintPagesPackCommand(id));
        }

    }
}