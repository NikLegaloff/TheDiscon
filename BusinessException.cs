using System;

namespace YAShop.BusinessLogic
{
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message)
        {
        }
    }
}