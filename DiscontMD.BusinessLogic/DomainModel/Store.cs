using System;
using System.Collections.Generic;
using System.Data;
using DiscontMD.BusinessLogic.Presistense;
using DiscontMD.BusinessLogic.Presistense.MSSQL;

namespace DiscontMD.BusinessLogic.DomainModel
{
    public enum DiscountType
    {
        EveryNForFree,
        Accumulative
    }
    public class StoreSettings
    {
        
        public string ShortName{ get; set; }
        public string DiscountDescription{ get; set; }
        public string NewsLine{ get; set; }
        public string Contacts{ get; set; }
        public DiscountType Type { get; set; }
        public int? EneryN { get; set; }
        public string AccumulativeRules { get; set; }
    }

    public class Store : DomainObject
    {

        [DBField(SqlDbType.NVarChar, 128)]
        public string DOmainKeyword{ get; set; }

        [DBField(SqlDbType.NVarChar, 0, false, true, typeof(StoreSettings))]
        public StoreSettings Settings { get; set; }

        private string SettingsJSON { get; set; }
    }
}