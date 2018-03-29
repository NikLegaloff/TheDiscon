using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DiscontMD.BusinessLogic;
using DiscontMD.BusinessLogic.Infrastr;
using YASop.AdminUI.Code;

namespace DiscontMD.WebUI
{
    public class WebCommonInfrastructureProvider : ICommonInfrastructureProvider
    {
        public object GetFromSession(string key)
        {
            return HttpContext.Current.Session[key];
        }

        public void PutInSession(string key, object subj)
        {
            HttpContext.Current.Session.Add(key, subj);
        }

        public IDictionary IdentityMap => HttpContext.Current.Items;
        public string CurrentDomainName()
        {
            return HttpContext.Current.Request.Url.Host;
        }
    }

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception lastError = Server.GetLastError();
            Exception ex = lastError.InnerException;
            if (lastError is RequireLoginException || ex != null && ex is RequireLoginException)
            {
                Response.Redirect("/Account/Login");
            }
        }
        protected void Application_Start()
        {
            Registry.Init(new WebCommonInfrastructureProvider());
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
