using System;
using System.Data;
using DiscontMD.BusinessLogic.Presistense;
using DiscontMD.BusinessLogic.Presistense.MSSQL;

namespace DiscontMD.BusinessLogic.DomainModel
{
    public class AvailableCard : DomainObject
    {
        [DBField(SqlDbType.UniqueIdentifier)]
        public Guid StoreId { get; set; }
        [DBField(SqlDbType.Int)]
        public int Num { get; set; }
    }
    public class CardPack : DomainObject
    {
        [DBField(SqlDbType.UniqueIdentifier, 0,true)]
        public Guid? StoreId { get; set; }
        [DBField(SqlDbType.Int)]
        public int NumBase { get; set; }
    }
}