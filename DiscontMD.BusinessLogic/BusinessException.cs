using System;

namespace DiscontMD.BusinessLogic
{
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message)
        {
        }
    }
}