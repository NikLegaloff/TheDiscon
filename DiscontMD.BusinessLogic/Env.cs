using System;

namespace DiscontMD.BusinessLogic
{
    public class Env
    {
        private EnvType _type;

        public Env()
        {
            var lower = AppDomain.CurrentDomain.BaseDirectory.ToLower();
            if (lower.Contains("prerelease")) _type = EnvType.Prerelease;
            else if (lower.Contains("live")) _type = EnvType.Live;
            else _type = EnvType.Dev;
        }

        public string ImagesPath
        {
            get { return "J:\\Projects\\DiscontMD\\DiscontMD.WebUI\\Img\\Pack\\"; }
        }
        public string MsSqlConnectionString
        {
            get
            {
                return "Data Source=.;Initial Catalog=DiscontMD;Persist Security Info=True;User ID=sa;Password=Password1";
            }
        }
    }
}