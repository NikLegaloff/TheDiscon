using System.Collections;
using System.Collections.Generic;
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
}