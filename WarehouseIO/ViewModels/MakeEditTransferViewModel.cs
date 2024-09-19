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

        public Transfer Transfer { get; set; }

        public int LastChangedToWarehouseId {get; set; }


        public MakeEditTransferViewModel()
        {
            
        }


        public MovingItem? GetAppropriateMovingItem(MovingItemViewModel item)
        {
            return this.Transfer.TransferItems
                .FirstOrDefault(i => i.Item.Name == item.Name);
        }

    }
}