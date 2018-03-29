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
        Dictionary<string,object> _identityMap = new Dictionary<string, object>();
        Dictionary<string,object> _session = new Dictionary<string, object>();
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

            Store store = new Store
            {
                DomainKeyword = "avtomoika"
            };

            store.Settings.AccumulativeRules = "0-100:3\n100-200:4\n200-500:5\n500-1000:6\n1000-5000:7%\n5000-:8%";
            store.Settings.Type=DiscountType.Accumulative;
            store.Settings.ShortName = "Автомойка";
             Registry.Current.Data.Stores.Save(store).Wait();
             Registry.Current.Services.Store.ActivateCard(111111, "Николай", "niklegaloff@gmail.com").Wait();
        }
    }
}
