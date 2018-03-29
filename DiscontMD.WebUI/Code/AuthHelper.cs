using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using DiscontMD.BusinessLogic;
using DiscontMD.BusinessLogic.DomainModel;

namespace YASop.AdminUI.Code
{
    public class AuthHelper
    {
        private static string SessionKey = "LoggedUser";
        public bool IsAuthenticated => Session[SessionKey] != null;

        private static HttpSessionState Session => HttpContext.Current.Session;

        public void LogOff()
        {
            Session[SessionKey] = null;
        }
        public void Authenticate(Guid id)
        {
            Session[SessionKey] = id.ToString();
        }
        public User CurrentUser()
        {
            var s = Session[SessionKey] as string;
            if (string.IsNullOrWhiteSpace(s)) HttpContext.Current.Response.Redirect("/Account/Login/");
            return AsyncHelpers.RunSync(() => Registry.Current.Data.Users.Find(new Guid(s)));
        }

    }
}