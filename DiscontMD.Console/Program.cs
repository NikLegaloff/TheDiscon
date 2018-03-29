using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscontMD.BusinessLogic;
using DiscontMD.BusinessLogic.DomainModel;
using DiscontMD.BusinessLogic.Infrastr;

namespace DiscontMD.Console
{
    public class ConsoleCommonInfrastructureProvider : ICommonInfrastructureProvider
    {
        readonly Dictionary<string,object> _identityMap = new Dictionary<string, object>();
        readonly Dictionary<string,object> _session = new Dictionary<string, object>();
        public object GetFromSession(string key)
        {
            return _session;
        }

        public void PutInSession(string key, object subj)
        {
            _session.Add(key, subj);
        }

        public IDictionary IdentityMap => _identityMap;
        public string CurrentDomainName()
        {
            return "http://avtomoika.diskont-md.com";
        }
    }

    class Program
    {
        static void  Main(string[] args)
        {
            Registry.Init(new ConsoleCommonInfrastructureProvider());

            Do3();
        }

        private static void Do3()
        {
            Registry.Current.Services.Card.AssignPackToStore(1, new Guid("BF1A81D8-BA91-46AA-82EB-973586485A44")).Wait();
        }
        private static void Do2()
        {
            List<int> list = new List<int>();
            for (int i = 1000; i < 10000; i++)
            {
                list.Add(i);
            }
            var MyArray = list.ToArray();
            Random rnd = new Random();
            int[] MyRandomArray = MyArray.OrderBy(x => rnd.Next()).ToArray();
            int[] MyRandomArray2 = MyRandomArray.OrderBy(x => rnd.Next()).ToArray();
            foreach (var i in MyRandomArray2)
            {
                Registry.Current.Data.CardPacks.Save(new CardPack {NumBase = i}).Wait();
            }
        }
        private static void Do1()
        {
            Store store = new Store
            {
                DomainKeyword = "avtomoika"
            };

            store.Settings.AccumulativeRules = new List<PriceRule>
            {
                new PriceRule {From = 0, Discount = 2},
                new PriceRule {From = 1000, Discount = 3},
                new PriceRule {From = 2000, Discount = 4},
                new PriceRule {From = 5000, Discount = 5},
                new PriceRule {From = 10000, Discount = 7},
            };
            store.Settings.Type = DiscountType.Accumulative;
            store.Settings.ShortName = "Автомойка";
            Registry.Current.Data.Stores.Save(store).Wait();
            Registry.Current.Services.Card.ActivateCard(111111, "Николай", "niklegaloff@gmail.com").Wait();
        }
    }
}
