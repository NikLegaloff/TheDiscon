using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DiscontMD.BusinessLogic.Presistense;
using DiscontMD.BusinessLogic.Presistense.MSSQL;

namespace DiscontMD.BusinessLogic.DomainModel
{
    public enum DiscountType
    {
        Accumulative, EveryNForFree
    }

    public class PriceRule
    {
        public PriceRule() { }

        public PriceRule(int @from, int discount)
        {
            From = @from;
            Discount = discount;
        }

        public  int From { get; set; }
        public  int Discount { get; set; }
    }
    public class StoreSettings
    {
        public StoreSettings()
        {
            AccumulativeRules=new List<PriceRule>();
        }

        public string ShortName{ get; set; }
        public string DiscountDescription{ get; set; }
        public string NewsLine{ get; set; }
        public string Contacts{ get; set; }
        public DiscountType Type { get; set; }
        public int? EneryN { get; set; }
        public List<PriceRule> AccumulativeRules { get; set; }
        public bool CountFirstTransaction{ get; set; }

    }

    public class Store : DomainObject, IWithEmbededProperty
    {
        public Store()
        {
            Settings=new StoreSettings();
        }

        [DBField(SqlDbType.NVarChar, 128)]
        public string DomainKeyword{ get; set; }

        [DBField(SqlDbType.NVarChar, 0, false, true, typeof(StoreSettings))]
        public StoreSettings Settings { get; set; }

        private string SettingsJSON { get; set; }

    }
}