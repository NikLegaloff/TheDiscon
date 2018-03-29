namespace DiscontMD.BusinessLogic.Infrastr
{
    public class Infrastructure
    {
        public Infrastructure(ICommonInfrastructureProvider common)
        {
            Common = common;
        }


        public ICommonInfrastructureProvider Common { get; }
    }
}