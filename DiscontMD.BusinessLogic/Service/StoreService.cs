using System;
using System.Linq;
using System.Threading.Tasks;
using DiscontMD.BusinessLogic.DomainModel;

namespace DiscontMD.BusinessLogic.Service
{
    public class ActivationCardResult
    {

        public bool IsOk => string.IsNullOrWhiteSpace(Error);
        public Card Card { get; set; }
        public string Error{ get; set; }
    }
    public class StoreService : AbstractService
    {
        public async Task<Store> CurrentStore()
        {
            if (Registry.Current.Infrastructure.Common.IdentityMap.Contains("CurrentStore")) return (Store) Registry.Current.Infrastructure.Common.IdentityMap["CurrentStore"];
            var currentStore = await LoadCurrentStore();
            Registry.Current.Infrastructure.Common.IdentityMap["CurrentStore"] = currentStore;
            return currentStore;
        }

        private static async Task<Store> LoadCurrentStore()
        {
            if (Registry.Current.Services.User.IsAuthenticated)
            {
                var user = Registry.Current.Services.User.CurrentUser();
                if (user.StoreId != null) return await Registry.Current.Data.Stores.Find(user.StoreId.Value);
            }
            var name = Registry.Current.Infrastructure.Common.CurrentDomainName();
            var ss = name.Split('.');
            if (ss.Length == 2) return null;
            var key = ss[0].Replace("http://", "");
            var list = await Registry.Current.Data.Stores.Select(" where DomainKeyword = @key", new {key});
            if (list == null || list.Length == 0) return null;
            return list[0];
        }

        public decimal DiscountFor(Card card)
        {
            var store = AsyncHelpers.RunSync(CurrentStore);
            if (store == null) return 0;
            var amount = card.Data.Amount;
            return store.Settings.AccumulativeRules.Max(r => r.From <= amount ? r.Discount : 0);
        }
    }
}