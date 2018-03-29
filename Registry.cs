using System;
using YAShop.BusinessLogic.Bus;
using YAShop.BusinessLogic.Infrastr;
using YAShop.BusinessLogic.Presistense;
using YAShop.BusinessLogic.Service;

namespace YAShop.BusinessLogic
{
    public enum EnvType
    {
        Devel, Test, Live
    }

    public class Env
    {
        private EnvType _type;

        public Env(EnvType type)
        {
            _type = type;
        }

        public string MsSqlConnectionString
        {
            get
            {
                return "Data Source=.;Initial Catalog=YAShop;Persist Security Info=True;User ID=sa;Password=Password1";
            }
        }
    }

    public class Registry
    {
        private static Registry _current;

        public static Registry Current
        {
            get
            {
                if (_current == null) throw new Exception("Not initialized");
                return _current;
            }
        }

        public Env Env{ get; set; }
        public Infrastructure Infrastructure { get; set; }
        public Services Services { get; set; }
        public DataProviders Data { get; set; }
        public CommandBus Bus { get; set; }

        public static void Init(ICommonInfrastructureProvider commonInfrastructureProvider)
        {
            _current = new Registry();
            _current.Env = new Env(EnvType.Devel);
            _current.Infrastructure = new Infrastructure(commonInfrastructureProvider);
            _current.Services = Services.Create();
            _current.Data = new DataProviders();
            _current.Bus = new CommandBus();
            
        }
    }
}