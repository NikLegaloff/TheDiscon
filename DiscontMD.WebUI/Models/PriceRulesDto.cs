using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DiscontMD.BusinessLogic.DomainModel;

namespace DiscontMD.WebUI.Models
{
    public class ViewCardResult
    {
        public  Card Card { get; set; }
        public  string Error{ get; set; }
    }
    public class PriceRulesDto
    {
        public int From0 { get; set; }
        public int From1 { get; set; }
        public int From2 { get; set; }
        public int From3 { get; set; }
        public int From4 { get; set; }
        public int From5 { get; set; }
        public int From6 { get; set; }
        public int From7 { get; set; }
        public int From8 { get; set; }
        public int Amount0 { get; set; }
        public int Amount1 { get; set; }
        public int Amount2 { get; set; }
        public int Amount3 { get; set; }
        public int Amount4 { get; set; }
        public int Amount5 { get; set; }
        public int Amount6 { get; set; }
        public int Amount7 { get; set; }
        public int Amount8 { get; set; }

    }
}