using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Dapper;
using DiscontMD.BusinessLogic;
using DiscontMD.BusinessLogic.DomainModel;
using DiscontMD.BusinessLogic.Presistense;
using DiscontMD.BusinessLogic.Presistense.MSSQL;
using DiscontMD.BusinessLogic.Service;
using DiscontMD.WebUI.Models;
using YASop.AdminUI.Code;

namespace DiscontMD.WebUI.Controllers
{
    public class BackofficeController : DiscontController
    {
        protected override bool RequireLogin => true;

        public ActionResult Index()
        {
            var storeId = Registry.Current.Services.User.CurrentUser().StoreId;
            if (storeId == null)
            {
                return RedirectToAction("CreateStore");
            }
            var dash = new BackofficeDash();
            using (var connection = MSSqlDb.Open())
            {
                dash.ActiveCards = (int) connection.ExecuteScalar("select COUNT(*) from Card where StoreId=@storeId", new {storeId=storeId.Value});
                dash.AvailableCards = (int) connection.ExecuteScalar("select COUNT(*) from AvailableCard where StoreId=@storeId", new {storeId=storeId.Value});
            }

            return View(dash);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public async Task<ActionResult> EditStore()
        {
            Store store;
            var storeId = Registry.Current.Services.User.CurrentUser().StoreId;
            if (storeId == null)
            {
                store = new Store {};
            }
            else store = await Registry.Current.Data.Stores.Find(storeId.Value);
            return View(store);
        }
        [AcceptVerbs(HttpVerbs.Post), ValidateInput(false)]
        public async Task<ActionResult> EditStore(string newsLine, string contacts)
        {
            var store = await Registry.Current.Services.Store.CurrentStore();
            store.Settings.NewsLine = newsLine;
            store.Settings.Contacts = contacts;
            await Registry.Current.Data.Stores.Save(store);
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> ActivateCard(string cardNum)
        {
            int num;
            if (!int.TryParse(cardNum.Trim().Replace(" ", ""), out num)) return View((Card)null);
            var res = await Registry.Current.Services.Card.ActivateCard(num, "", "");
            if (res.IsOk) return RedirectToAction("ViewCard", new {cardNum = num.ToString()});
            return View(res);
        }

        public async Task<ActionResult> ViewCard(string cardNum)
        {
            int num;
            if (!int.TryParse(cardNum.Trim().Replace(" ", ""), out num)) return View((Card)null);
            var card = await Registry.Current.Services.Card.FindByNum(num);
            if (card!=null) card.Data.Items.Sort();
            var viewCardResult = new ViewCardResult {Card = card};
            if (card == null)
            {
                viewCardResult.Error = "не найдена";
                if (await Registry.Current.Services.Card.IsNotActivated(num)) viewCardResult.Error = "не активирована";
            }
            return View(viewCardResult);
        }

        public ActionResult UpdateCardDetails()
        {
            throw new System.NotImplementedException();
        }

        public async Task<ActionResult> AddTransaction(int cardNum, decimal amount, string comment)
        {
            await Registry.Current.Services.Card.RegisterOperation(cardNum, amount, comment);
            return RedirectToAction("ViewCard",new {cardNum});
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public async Task<ActionResult> ListActive()
        {
            var storeId = Registry.Current.Services.User.CurrentUser().StoreId.Value;

            var list = await Registry.Current.Data.Cards.Select(" where storeId=@storeId", new {storeId});

            if (list != null)
            {
                var tmp = new List<Card>(list);
                tmp.Sort();
                list = tmp.ToArray();
            }
            return View(list);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public async Task<ActionResult> CreateStore(string error = null)
        {
            dynamic model = new ExpandoObject();
            model.Error = error;
            return View(model);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> CreateStore(string name, string domain, int type,int everyN, PriceRulesDto prices)
        {
            var byDomain = await Registry.Current.Data.Stores.Select(" where domain=@domain", new { domain });
            if (byDomain != null && byDomain.Length > 0) return RedirectToAction("CreateStore", new {error = "Доменное имя '" + domain + "' занято"});

            byDomain = await Registry.Current.Data.Stores.Select(" where name=@name", new { name});
            if (byDomain != null && byDomain.Length > 0) return RedirectToAction("CreateStore", new {error = "Имя сайта '" + name + "' занято"});

            Store store = new Store {Domain= domain};
            store.Settings.Type = (DiscountType)type;
            store.Name = name;
            if (store.Settings.Type == DiscountType.EveryNForFree) store.Settings.EneryN = everyN;
            else
            {
                var list = new List<PriceRule>();
                if (prices.Amount0>0) list.Add(new PriceRule (prices.From0, prices.Amount0));
                if (prices.Amount1>0 && prices.From1>0) list.Add(new PriceRule (prices.From1, prices.Amount1));
                if (prices.Amount2>0 && prices.From2>0) list.Add(new PriceRule (prices.From2, prices.Amount2));
                if (prices.Amount3>0 && prices.From3>0) list.Add(new PriceRule (prices.From3, prices.Amount3));
                if (prices.Amount4>0 && prices.From4>0) list.Add(new PriceRule (prices.From4, prices.Amount4));
                if (prices.Amount5>0 && prices.From5>0) list.Add(new PriceRule (prices.From5, prices.Amount5));
                if (prices.Amount6>0 && prices.From6>0) list.Add(new PriceRule (prices.From6, prices.Amount6));
                if (prices.Amount7>0 && prices.From7>0) list.Add(new PriceRule (prices.From7, prices.Amount7));
                if (prices.Amount8>0 && prices.From8>0) list.Add(new PriceRule (prices.From8, prices.Amount8));

                store.Settings.AccumulativeRules=list;
            }
            await Registry.Current.Data.Stores.Save(store);
            var currentUser = Registry.Current.Services.User.CurrentUser();
            currentUser.StoreId = store.Id;
            await Registry.Current.Data.Users.Save(currentUser);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> CheckDomainName(string domainName)
        {
            return null;
        }
    }
}