using System;
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
    }
}