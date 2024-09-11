using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WarehouseIO.Models;

namespace WarehouseIO.ViewModels
{
    public class AddEditItemViewModel
    {
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public ItemType Type { get; set; }

        public ICollection<ItemType> AllItemTypes { get; set; } = new List<ItemType>();
        public string? ImageUrl { get; set; }
        [Required]
        public double EstPrice { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        public int Size { get; set; }

        public string ErrorMessage { get; set; }

        public AddEditItemViewModel()
        {
            
        }

        public AddEditItemViewModel(Warehouse warehouse)
        {
            this.Warehouse = warehouse;
            this.WarehouseId = warehouse.Id;
        }

        public AddEditItemViewModel(Warehouse warehouse, Item item)
        {
            this.Warehouse = warehouse;
            this.WarehouseId = warehouse.Id;
            this.Id = item.Id;
            this.Name = item.Name;
            this.Description = item.Description;
            this.Type = item.Type;
            this.EstPrice = item.EstPrice;
            this.Amount = item.Amount;
            this.Size = item.Size;
            this.ImageUrl = item.ImageUrl;
        }

        public static List<string> GetPropertiesNames()
        {
            return new List<string>() { nameof(Name), nameof(Description), nameof(Type), nameof(EstPrice), nameof(Amount), nameof(Size) };
        }
    }
}