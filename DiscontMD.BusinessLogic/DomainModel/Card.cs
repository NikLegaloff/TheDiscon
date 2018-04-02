using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DiscontMD.BusinessLogic.Presistense;
using DiscontMD.BusinessLogic.Presistense.MSSQL;

namespace DiscontMD.BusinessLogic.DomainModel
{
    public class CardTransaction : IComparable<CardTransaction>
    {
        public DateTime Date { get; set; }
        public Decimal Amount { get; set; }
        public string Comment{ get; set; }

        public int CompareTo(CardTransaction other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return -1;
            return -Date.CompareTo(other.Date);
        }
    }

    public class CardData
    {
        public CardData()
        {
            Items=new List<CardTransaction>();
        }

        public string ContactName { get; set; }
        public string ContactAddress { get; set; }
        public string Comments { get; set; }
        public DateTime ActivationDate{ get; set; }
        public int Counter => Items.Sum(i => (int)i.Amount);

        public Decimal Amount => Items.Sum(i => i.Amount);

        public List<CardTransaction> Items { get; set; }

    }

    public class Card : DomainObject, IWithEmbededProperty, IComparable<Card>
    {
        public Card()
        {
            Data=new CardData();
        }

        [DBField(SqlDbType.Int)]
        public int Num{ get; set; }
        public string FormattedNum => Num.ToString().Insert(3, "&nbsp;");

        [DBField(SqlDbType.UniqueIdentifier)]
        public Guid StoreId { get; set; }

        [DBField(SqlDbType.NVarChar, 0, false, true, typeof(CardData))]
        public CardData Data { get; set; }
        private string DataJSON { get; set; }

        public Task<Store> Store => Registry.Current.Data.Stores.Find(StoreId);

        public int CompareTo(Card other)
        {
            return -Data.ActivationDate.CompareTo(other.Data.ActivationDate);
        }
    }
}