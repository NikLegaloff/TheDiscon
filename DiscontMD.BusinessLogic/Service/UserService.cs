using System;
using System.Threading.Tasks;
using DiscontMD.BusinessLogic.DomainModel;

namespace DiscontMD.BusinessLogic.Service
{
    public class UserService : AbstractService
    {
        public async Task<bool> ChangePassword(string email, string oldPass, string newPass)
        {
            var existing = await Get(email, oldPass);
            if (existing==null) throw new BusinessException("Email or old password is wrong");
            existing.Password = newPass;
            await Registry.Current.Data.Users.Save(existing);
            return true;
        }
        public async Task<User> Register(string email, string pass, UserRole role)
        {
            var users = await Registry.Current.Data.Users.Select(" where Email=@email", new { email});
            if (users.Length>0) throw new BusinessException("User email is not unique");
            var user = new User {Email = email,Password = pass,Role = role};
            await Registry.Current.Data.Users.Save(user);
            return user;

        }
        public async Task<User> Get(string email, string pass)
        {
            var users = await Registry.Current.Data.Users.Select(" where Email=@email and Password=@pass", new {email, pass});
            return users.Length == 0 ? null : users[0];
        }
        public async Task<User> Get(string email)
        {
            var users = await Registry.Current.Data.Users.Select(" where Email=@email", new {email});
            return users.Length == 0 ? null : users[0];
        }

        private static string SessionKey = "LoggedUser";
        public bool IsAuthenticated => (Registry.Current.Infrastructure.Common.GetFromSession(SessionKey) as string) != null;


        public void LogOff()
        {
            Registry.Current.Infrastructure.Common.PutInSession(SessionKey, null);
        }
        public void Authenticate(Guid id)
        {
            Registry.Current.Infrastructure.Common.PutInSession(SessionKey, id.ToString()); 
        }
        public User CurrentUser()
        {
            var s = Registry.Current.Infrastructure.Common.GetFromSession(SessionKey) as string;
            //if (string.IsNullOrWhiteSpace(s)) HttpContext.Current.Response.Redirect("/Account/Login/");
            return AsyncHelpers.RunSync(() => Registry.Current.Data.Users.Find(new Guid(s)));
        }

    }
}