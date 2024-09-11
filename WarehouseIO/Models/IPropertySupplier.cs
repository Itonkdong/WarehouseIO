using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseIO.Models
{
    public interface IPropertySupplier
    {
        List<string> GetPropertiesNames();
    }
}