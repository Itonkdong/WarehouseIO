using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WarehouseIO.Models;

namespace WarehouseIO.ViewModels
{
    public class ManageShipmentsViewModel : AViewModel
    {
        public List<Warehouse> AllWarehouses { get; set; } = new List<Warehouse>();

        public List<Shipment> Shipments { get; set; } = new List<Shipment>();

        public int WarehouseId { get; set; }    
        public Warehouse Warehouse { get; set; }

        public ManageShipmentsViewModel()
        {
            
        }
    }
}