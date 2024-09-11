using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WarehouseIO.Models;

namespace WarehouseIO.ViewModels
{
    public class MakeEditTransferViewModel
    {
        public int FromWarehouseId { get; set; }
        public int ToWarehouseId { get; set; }
        public Warehouse FromWarehouse { get; set; }
        public Warehouse ToWarehouse { get; set; }
        public List<MovingItemViewModel> AllFromWarehouseItems { get; set; } = new List<MovingItemViewModel>();
        public ICollection<Warehouse> AllWarehouses { get; set; } = new List<Warehouse>();
        public string? ErrorMessage { get; set; }

        public MakeEditTransferViewModel()
        {
            
        }

    }
}