using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace DiscontMD.BusinessLogic.Presistense.MSSQL
{
    public class MSSqlDb
    {
        public static SqlConnection Open()
        {
            var connection = new SqlConnection(Registry.Current.Env.MsSqlConnectionString);
            connection.Open();
            return connection;
        }

        public static async Task Execute(string sql, object param = null)
        {
            using (var db = Open())
            {
                await db.ExecuteAsync(sql, param);
            }
        }

        public static async Task<T> ExecuteScalar<T>(string sql, object param = null)
        {
            using (var db = Open())
            {
                return await db.ExecuteScalarAsync<T>(sql, param);
            }
        }
    }
}