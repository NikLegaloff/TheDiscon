using System;
using DiscontMD.BusinessLogic.Bus;
using DiscontMD.BusinessLogic.Infrastr;
using DiscontMD.BusinessLogic.Presistense;
using DiscontMD.BusinessLogic.Service;

namespace DiscontMD.BusinessLogic
{
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
            _current.Env = new Env(EnvType.Dev);
            _current.Infrastructure = new Infrastructure(commonInfrastructureProvider);
            _current.Services = Services.Create();
            _current.Data = new DataProviders();
            _current.Bus = new CommandBus();
            
        }
    }
}