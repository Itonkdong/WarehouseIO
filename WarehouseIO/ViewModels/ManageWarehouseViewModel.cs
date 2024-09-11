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

        /*
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string Location { get; set; }

        public long MaxCapacity { get; set; }

        public List<Item> StoredItems { get; set; } = new List<Item>();

        [Display(Name = "Capacity")]
        public long CurrentCapacity =>
            StoredItems.ToList()
                .Select(item => item.Size)
                .Sum() / 1000;
                */

        public ManageWarehouseViewModel()
        {
            
        }

        public ManageWarehouseViewModel(Warehouse warehouse)
        {
            this.Warehouse = warehouse;
            this.WarehouseId = warehouse.Id;
            /*this.Name = warehouse.Name;
            this.Description = warehouse.Description;
            this.Location = warehouse.Location;
            this.MaxCapacity = warehouse.MaxCapacity;
            this.Id = warehouse.Id;
            this.StoredItems = warehouse.StoredItems.ToList();*/
        }
    }
}