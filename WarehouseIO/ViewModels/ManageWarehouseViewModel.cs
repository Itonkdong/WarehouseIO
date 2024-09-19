using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WarehouseIO.Models;

namespace WarehouseIO.ViewModels
{
    public class ManageWarehouseViewModel
    {
        public ICollection<Warehouse> AllWarehouses { get; set; } = new List<Warehouse>();
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }

        public ManageWarehouseViewModel()
        {
            
        }

        public ManageWarehouseViewModel(Warehouse warehouse)
        {
            this.Warehouse = warehouse;
            this.WarehouseId = warehouse.Id;
        }

        public bool IsUserManagerInWarehouse(string userEmail)
        {
            return this.Warehouse
                .Managers
                .Any(m => m.Email == userEmail);
        }
    }
}