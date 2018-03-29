using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace DiscontMD.BusinessLogic.Presistense.MSSQL
{
    public class JsonFilesDataProvider<T> : IDataProvider<T> where T : DomainObject, new()
    {
        
        string basePath;

        string PathForId(Guid id)
        {
            return basePath + id + ".json";
        }
        public async Task<T> Find(Guid id)
        {
            if (!File.Exists(PathForId(id))) return null;
            var str = File.ReadAllText(PathForId(id));
            return JsonConvert.DeserializeObject<T>(str);

        }

        public async Task Save(T subj)
        {
            if (subj.Id==Guid.Empty) subj.Id=Guid.NewGuid();
            File.WriteAllText(PathForId(subj.Id), JsonConvert.SerializeObject(subj));
        }

        public async Task Delete(T subj)
        {
            if (File.Exists(PathForId(subj.Id))) File.Delete(PathForId(subj.Id));
        }

        public async Task<T[]> Select(Func<T, bool> filter)
        {
            var list = new List<T>();
            foreach (var file in Directory.GetFiles(basePath))
            {
                var subj = JsonConvert.DeserializeObject<T>(File.ReadAllText(file));
                if (filter(subj)) list.Add(subj);
            }
            return list.ToArray();
        }

        public Task<T[]> Select(string sql = null, dynamic param = null, int? top = null)
        {
            throw new NotImplementedException();
        }

        public Task<PageData<T>> SelectPage(string query, PagingArgs paging, dynamic param = null, string sortingAlias = null,
            string extraSorting = null)
        {
            throw new NotImplementedException();
        }

        public IDataProvider<T> Init(dynamic options)
        {
            basePath = options.BasePath;
            basePath = basePath.Trim('\\') + "\\" + typeof(T).Name + "\\";
            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
            return this;
        }
    }
}