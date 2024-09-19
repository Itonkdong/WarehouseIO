using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WarehouseIO.Models;

namespace WarehouseIO.ViewModels
{
    public class DetailsTransferHistoryViewModel
    {
        public Warehouse FromWarehouse { get; set; }
        public Warehouse ToWarehouse { get; set; }

        public Transfer Transfer { get; set; }

        public string? ErrorMessage { get; set; }

        public DetailsTransferHistoryViewModel()
        {
            
        }
    }
}