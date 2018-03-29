using System;
using System.Threading.Tasks;
using DiscontMD.BusinessLogic.DomainModel;

namespace DiscontMD.BusinessLogic.Service
{
    public class StoreService : AbstractService
    {
        public async Task<Store> CurrentStore()
        {
            var name = Registry.Current.Infrastructure.Common.CurrentDomainName();
            var ss = name.Split('.');
            if (ss.Length == 2) return null;
            var key = ss[0].Replace("http://", "");
            var list = await Registry.Current.Data.Stores.Select(" where DomainKeyword = @key", new {key});
            if (list == null || list.Length == 0) return null;
            return list[0];
        }
        public async Task<Card> ActivateCard(int num, string name, string addreds)
        {
            var store = await CurrentStore();
            var card = new Card { StoreId = store.Id, Num = num };
            card.Data.ActivationDate = DateTime.Now;
            card.Data.ContactName = name;
            card.Data.ContactAddress = addreds;
            await Registry.Current.Data.Cards.Save(card);
            return card;
        }

        public decimal DiscountFor(Card model)
        {
            return 5m;
        }
    }
}