using System.Collections;

namespace DiscontMD.BusinessLogic.Infrastr
{
    public interface ICommonInfrastructureProvider
    {
        IDictionary IdentityMap { get; }
        object GetFromSession(string key);
        void PutInSession(string key, object subj);
        string CurrentDomainName();
    }
}