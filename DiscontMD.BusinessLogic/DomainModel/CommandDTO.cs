using System;
using System.Data;
using DiscontMD.BusinessLogic.Presistense;
using DiscontMD.BusinessLogic.Presistense.MSSQL;

namespace DiscontMD.BusinessLogic.DomainModel
{
    public enum CommandSatet
    {
        Ready,
        Executing,
        Done,
        Failed
    }

    public class CommandDTO : DomainObject
    {
        [DBField(SqlDbType.UniqueIdentifier, 0, true)]
        public Guid? TakeId { get; set; }

        [DBField(SqlDbType.NText, 128)]
        public string BodyJSON { get; set; }

        [DBField(SqlDbType.Int)]
        public CommandSatet State { get; set; }

        [DBField(SqlDbType.NText, 0, true)]
        public string Status { get; set; }

        [DBField(SqlDbType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [DBField(SqlDbType.DateTime, 0, true)]
        public DateTime? StartedDate { get; set; }

        [DBField(SqlDbType.DateTime, 0, true)]
        public DateTime? EndedDate { get; set; }
    }
}