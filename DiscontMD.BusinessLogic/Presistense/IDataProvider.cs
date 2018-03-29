using System;
using System.Threading.Tasks;
using DiscontMD.BusinessLogic.Presistense.MSSQL;

namespace DiscontMD.BusinessLogic.Presistense
{
    public interface IDataProvider<T> where T : DomainObject, new()
    {
        IDataProvider<T> Init(dynamic options=null);
        Task<T> Find(Guid id);
        Task Save(T subj);
        Task Delete(T subj);
        Task<T[]> Select(Func<T,bool> filter);
        Task<T[]> Select(string sql = null, dynamic param = null, int? top = null);

        Task<PageData<T>> SelectPage(string query, PagingArgs paging, dynamic param = null, string sortingAlias = null,
            string extraSorting = null);
    }
}