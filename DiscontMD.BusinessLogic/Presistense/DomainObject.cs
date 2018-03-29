using System;
using System.Linq;

namespace DiscontMD.BusinessLogic.Presistense
{
    public class DomainObject
    {
        public DomainObject()
        {
            Id = Guid.Empty;
        }

        public int __RowsTotal { get; set; }


        public Guid Id { get; set; }

        public override string ToString()
        {
            return GetType().Name + " " + Id + "\n" +
                   string.Join(", ",
                       GetType()
                           .GetProperties()
                           .Where(t => t.Name != "Id" && t.GetValue(this) != null)
                           .Select(t => t.Name + ": " + t.GetValue(this))
                           .Where(s => !s.Contains("List`1")));
        }
    }
}