using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;
using DiscontMD.BusinessLogic.DomainModel;

namespace DiscontMD.BusinessLogic.Presistense.MSSQL
{
    public interface IWithEmbededProperty
    {
    }
    public class MSSqlDataProvider<T> : IDataProvider<T> where T : DomainObject, new()
    {
        private readonly Dictionary<string, PropertyInfo> _jsonProps = new Dictionary<string, PropertyInfo>();

        public IDataProvider<T> Init(dynamic options=null)
        {
            var tableName = typeof(T).Name;
            using (var connection = MSSqlDb.Open())
            {
                var tid = connection.ExecuteScalar<int?>("select OBJECT_ID('" + tableName + "')");
                if (tid != null) return this;
                var props = typeof(T).GetProperties();
                var withNtext = false;
                var columns = new List<string>();
                foreach (var prop in props)
                {
                    var attr = prop.GetCustomAttribute<DBField>();
                    if (attr == null) continue;
                    var type = attr.DBType.ToString();
                    var nullable = attr.Nullable ? "NULL" : "NOT NULL";
                    var name = prop.Name;
                    var len = attr.Len > 0 ? "(" + attr.Len + ")" : "";
                    if (attr.DBType == SqlDbType.Decimal) len = "(9, 2)";
                    if (attr.InJson)
                    {
                        type = "ntext";
                        len = "";
                        name += "JSON";
                    }
                    if (attr.DBType == SqlDbType.NText)
                    {
                        withNtext = true;
                        len = "";
                    }
                    columns.Add($"{name} {type}{len} {nullable}");
                }
                var sql = "CREATE TABLE dbo.[" + tableName + "] (Id uniqueidentifier NOT NULL, " +
                          string.Join(", ", columns) + ")";
                if (withNtext) sql += " ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                connection.Execute(sql);
                sql = $"ALTER TABLE dbo.[{tableName}] ADD CONSTRAINT\tPK_{tableName} PRIMARY KEY CLUSTERED (Id)" +
                      $" WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]";

                connection.Execute(sql);
            }

            return this;
        }

        public async Task<T> Find(Guid id)
        {
            using (var sqlConnection = MSSqlDb.Open())
            {
                var find = await sqlConnection.QueryFirstAsync<T>("select * from [" + typeof(T).Name + "] where Id='" + id + "'");
                DeserializeProps(new[] {find});
                return find;
            }
        }

        public async Task Save(T subj)
        {
            if (subj.Id == Guid.Empty) await Insert(subj);
            else await Update(subj);
        }


        public async Task Delete(T subj)
        {
            using (var sqlConnection = MSSqlDb.Open())
            {
                await sqlConnection.ExecuteAsync("delete [" + subj.GetType().Name + "] where id='" + subj.Id + "'");
            }
        }

        public async Task<T[]> Select(string query = null, object param = null, int? top = null)
        {
            if (!(query ?? "").StartsWith("select")) query = "select " + (top == null ? "" : "TOP " + top.Value) + " [" + typeof(T).Name + "].* from [" + typeof(T).Name + "] (nolock) " + query;
            using (var sqlConnection = MSSqlDb.Open())
            {
                var result = await sqlConnection.QueryAsync<T>(query, param);
                var items = result.ToArray();
                DeserializeProps(items);
                return items;
            }
        }

        public async Task<PageData<T>> SelectPage(string query, PagingArgs paging, dynamic param = null,
            string sortingAlias = null, string extraSorting = null)
        {
            if (!(query ?? "").StartsWith("select")) query = "select * from[" + typeof(T).Name + "] (nolock)" + query;
            using (var c = MSSqlDb.Open())
            {
                if (string.IsNullOrWhiteSpace(paging.Sort))
                    throw new InvalidOperationException("Sort field is empty");

                if (param == null)
                    param = new ExpandoObject();

                var sort = string.IsNullOrWhiteSpace(paging.Sort)
                    ? GetFirstProperty()
                    : EnsurePropery(paging.Sort);

                var sortDir = paging.SortDir.ToString();

                query = query.Replace("select", string.Format("select " +
                                                              "     row_number() over ( order by {0} {1} {2}) as __RowNum, " +
                                                              "     count({0}) over () AS __RowsTotal,",
                    (string.IsNullOrWhiteSpace(sortingAlias) ? "" : sortingAlias + ".") + sort, sortDir, extraSorting ?? ""));

                var page = paging.Page == 0 ? 1 : paging.Page;

                var inputRowsPerPage = paging.RowsPerPage == 0 ? PagingArgs.DefaultRowsPerPage : paging.RowsPerPage;
                var rowsPerPage = Math.Min(inputRowsPerPage, 1000); // Defending input

                param.__PagingFrom = (page - 1) * rowsPerPage + 1;
                param.__PagingTo = page * rowsPerPage;

                query =
                    $@"select * from ( {query} ) as __Res where __RowNum between @__PagingFrom and @__PagingTo order by __RowNum";

                var queryRes = await c.QueryAsync<T>(query, (object) param);
                var queryPage = queryRes.ToArray();
                DeserializeProps(queryPage);

                var firstRow = queryPage.FirstOrDefault();
                var totalRows = firstRow?.__RowsTotal ?? 0;
                return new PageData<T>
                {
                    TotalRows = totalRows,
                    Rows = queryPage
                };
            }
        }

        private async Task Update(T subj)
        {
            var props =
                subj.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var pp = new ExpandoObject() as IDictionary<string, object>;
            var items = new List<string>();
            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttribute<DBField>();
                if (attr == null) continue;
                var value = prop.GetValue(subj);
                if (attr.InJson)
                {
                    var newJson = JsonConvert.SerializeObject(value);

                    PropertyInfo jsonProp;
                    if (_jsonProps.ContainsKey(prop.Name))
                    {
                        jsonProp = _jsonProps[prop.Name];
                    }
                    else
                    {
                        jsonProp = props.FirstOrDefault(p => p.Name == prop.Name + "JSON");
                        _jsonProps.Add(prop.Name, jsonProp);
                    }

                    var oldJson = (string) jsonProp.GetValue(subj);
                    // do not update same value
                    if (oldJson == newJson) continue;

                    pp.Add(prop.Name + "JSON", newJson);
                    items.Add(prop.Name + "JSON");
                }
                else
                {
                    pp.Add(prop.Name, value);
                    items.Add(prop.Name);
                }
            }
            var sql = "update [" + subj.GetType().Name + "] set " + string.Join(",", items.Select(i => i + "=@" + i)) +
                      " where id='" + subj.Id + "'";
            using (var sqlConnection = MSSqlDb.Open())
            {
                await sqlConnection.ExecuteAsync(sql, pp);
            }
        }

        private async Task Insert(T subj)
        {
            subj.Id = Guid.NewGuid();
            var props = subj.GetType().GetProperties();
            var pp = new ExpandoObject() as IDictionary<string, object>;
            pp.Add("Id", subj.Id);
            var items = new List<string> {"Id"};
            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttribute<DBField>();
                if (attr == null) continue;
                var value = prop.GetValue(subj);
                if (value == null) continue;
                if (attr.InJson)
                {
                    pp.Add(prop.Name + "JSON", JsonConvert.SerializeObject(value));
                    items.Add(prop.Name + "JSON");
                }
                else
                {
                    pp.Add(prop.Name, value);
                    items.Add(prop.Name);
                }
            }
            var sql = "insert into [" + subj.GetType().Name + "](" + string.Join(",", items) + ") values (" +
                      string.Join(",", items.Select(i => "@" + i)) + ")";
            using (var sqlConnection = MSSqlDb.Open())
            {
                await sqlConnection.ExecuteAsync(sql, pp);
            }
        }

        private void DeserializeProps(T[] items)
        {
            if (items.Length == 0 || !(items[0] is IWithEmbededProperty)) return;
            var allprops = typeof(T).GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var props = allprops.Where(p =>
            {
                var customAttribute = p.GetCustomAttribute<DBField>();
                return customAttribute != null && customAttribute.InJson;
            }).ToArray();

            foreach (var prop in props)
            {
                var customAttribute = prop.GetCustomAttribute<DBField>();
                var srcProp = allprops.FirstOrDefault(p => p.Name == prop.Name + "JSON");
                foreach (var item in items)
                {
                    var value = (string) srcProp.GetValue(item);
                    prop.SetValue(item, JsonConvert.DeserializeObject(value, customAttribute.Type));
                }
            }
        }

        private static string EnsurePropery(string prop)
        {
            var allProps = typeof(T).GetProperties();
            if (allProps.Any(x => x.Name == prop)) return prop;
            throw new InvalidOperationException("Invalid property: " + prop);
        }

        private static string GetFirstProperty()
        {
            var propertyInfo = typeof(T).GetProperties().FirstOrDefault();
            if (propertyInfo == null)
                throw new InvalidOperationException(
                    "Could not get the 1st property from the type. Set the 'sort' parameter explicitly");
            return propertyInfo.Name;
        }

        public Task<T[]> Select(Func<T, bool> filter)
        {
            throw new NotImplementedException();
        }
    }
}