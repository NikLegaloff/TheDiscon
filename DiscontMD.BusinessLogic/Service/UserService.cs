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
    }
}