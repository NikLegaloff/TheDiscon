using System;
using System.Threading;
using System.Threading.Tasks;
using DiscontMD.BusinessLogic.DomainModel;

namespace DiscontMD.BusinessLogic.Service
{
    public class CardService : AbstractService
    {
        public  async Task<Card> FindByNum(int num)
        {
            var cards = await Registry.Current.Data.Cards.Select(" where Num=@num", new { num});
            if (cards == null || cards.Length == 0) return null;
            return cards[0];
        }

        public  void AssignPackToStore(int count, Guid storeId)
        {
            var packs = AsyncHelpers.RunSync(() => Registry.Current.Data.CardPacks.Select(" where storeId is null", null, count));
            foreach (var pack in packs)
            {
                for (int i = 0; i < 100; i++)
                {
                    var card = new AvailableCard
                    {
                        StoreId = storeId,
                        Num = pack.NumBase * 100 + i
                    };
                    AsyncHelpers.RunSync(() => Registry.Current.Data.AvailableCards.Save(card));
                }
                pack.StoreId = storeId;
                AsyncHelpers.RunSync(() => Registry.Current.Data.CardPacks.Save(pack));
            }
        }
        public async Task<ActivationCardResult> ActivateCard(int num, string name, string addreds)
        {
            var existing = await FindByNum(num);
            if (existing != null) return new ActivationCardResult { Error = "Карта уже активирована", };
            var store = await Registry.Current.Services.Store.CurrentStore();
            var avilable = await Registry.Current.Data.AvailableCards.Select(" where Num=" + num + " and StoreId = @storeId", new { storeId = store.Id });
            if (avilable == null || avilable.Length == 0)
            {
                return new ActivationCardResult { Error = "Неизвестный номер карты", };
            };
            await Registry.Current.Data.AvailableCards.Delete(avilable[0]);

            var card = new Card { StoreId = store.Id, Num = num };
            card.Data.ActivationDate = DateTime.Now;
            card.Data.ContactName = name;
            card.Data.ContactAddress = addreds;
            await Registry.Current.Data.Cards.Save(card);
            return new ActivationCardResult { Card = card };
        }

        public async Task RegisterOperation(int num, decimal amount, string comment)
        {
            var card = await FindByNum(num);
            card.Data.Items.Add(new CardTransaction
            {
                Date = DateTime.Now,
                Amount = amount,
                Comment = comment
            });
            await Registry.Current.Data.Cards.Save(card);
        }

        public async Task<bool> IsNotActivated(int num)
        {
            var store = await Registry.Current.Services.Store.CurrentStore();
            var avilable = await Registry.Current.Data.AvailableCards.Select(" where Num=" + num + " and StoreId = @storeId", new { storeId =store.Id});
            return avilable != null && avilable.Length > 0;
        }
    }
}