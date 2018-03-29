using System;
using System.Data;

namespace DiscontMD.BusinessLogic.Presistense.MSSQL
{
    public class DBField : Attribute
    {
        public Func<string> GetJSON;

        public DBField(SqlDbType dbType, int len = 0, bool nullable = false, bool inJson = false, Type type = null)
        {
            Nullable = nullable;
            InJson = inJson;
            Type = type;
            DBType = dbType;
            Len = len;
        }

        public bool Nullable { get; }
        public bool InJson { get; }
        public Type Type { get; }
        public SqlDbType DBType { get; }
        public int Len { get; }
    }
}