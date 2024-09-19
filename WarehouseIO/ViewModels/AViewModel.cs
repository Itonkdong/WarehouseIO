using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseIO.ViewModels
{
    public abstract class AViewModel
    {
        public string? ErrorMessage { get; set; }
    }
}