
using DiscontMD.BusinessLogic.Service.Users;

namespace DiscontMD.BusinessLogic.Service
{
    public class Services
    {
        public UserService User { get; private set; }

        internal static Services Create()
        {
            return new Services
            {
                User = new UserService(),
            };
        }
    }
}