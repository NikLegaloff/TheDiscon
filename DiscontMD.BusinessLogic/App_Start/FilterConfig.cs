﻿using System.Web;
using System.Web.Mvc;

namespace DiscontMD.BusinessLogic
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
