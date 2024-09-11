using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseIO.Exceptions
{
    public class InvalidAmountValueException : Exception
    {
        public InvalidAmountValueException(string message) : base(message)
        {
            
        }
    }
}