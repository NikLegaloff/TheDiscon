namespace DiscontMD.BusinessLogic.Service
{
    public class Services
    {
        public UserService User { get; private set; }
        public StoreService Store { get; private set; }
        public CardService Card { get; private set; }

        internal static Services Create()
        {
            return new Services
            {
                User = new UserService(),
                Store = new StoreService(),
                Card = new CardService(),
            };
        }
    }
}