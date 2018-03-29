using System.Threading.Tasks;
using System.Web.Mvc;
using DiscontMD.BusinessLogic;
using DiscontMD.BusinessLogic.DomainModel;

namespace DiscontMD.WebUI.Controllers
{
    public class BackofficeController : DiscontController
    {
        protected override bool RequireLogin => true;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ActivateCard(int cardNum)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ActionResult> ViewCard(string cardNum)
        {
            int num;
            if (!int.TryParse(cardNum.Trim().Replace(" ", ""), out num)) return View((Card)null);
            var card = await Registry.Current.Services.Card.FindByNum(num);
            card.Data.Items.Sort();
            return View(card);
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
    }
}