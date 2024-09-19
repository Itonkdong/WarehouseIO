using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseIO.Models
{
    public class TryResult
    {
        public bool Result { get; set; }
        public Exception? Exception { get; set; }

        public TryResult(bool result)
        {
            this.Result = result;
        }

        public TryResult(bool result, Exception exception)
        {
            this.Result = result;
            this.Exception = exception;
        }
    }
}