using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using WarehouseIO.ViewModels;

namespace WarehouseIO.Models
{
    public class Warehouse
    {
        public int Id { get; set; }

        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        [MaxLength(200)]
        [Required]
        public string Description { get; set; }

        [MaxLength(100)]
        [Required]
        public string Location { get; set; }

        [Display(Name = "Max Capacity")]
        [Required]
        public long MaxCapacity { get; set; }

        [Display(Name = "Capacity")]
        public long CurrentCapacity => (long) Math.Ceiling(StoredItems.ToList()
            .Select(item => item.Size * item.Amount)
            .Sum() / 1000.0);

        [ForeignKey("WarehouseId")]
        public virtual ICollection<Item> StoredItems { get; set; } = new List<Item>();

        [ForeignKey("FromWarehouseId")]
        public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();

        public virtual ICollection<Transfer> TransfersFromWarehouse { get; set; } = new List<Transfer>();

        public virtual ICollection<Transfer> TransfersToWarehouse { get; set; } = new List<Transfer>();

        public virtual ICollection<ApplicationUser> Operators { get; set; } = new List<ApplicationUser>();
        public virtual ICollection<ApplicationUser> Managers { get; set; } = new List<ApplicationUser>();

        public Warehouse()
        { 

            
        }

        public List<MovingItemViewModel> GetMovingItemViewModels()
        {
            return this.StoredItems
                .Select(item => new MovingItemViewModel(item))
                .ToList();
        }

        public static List<string> GetPropertiesNames()
        {
            return new List<string>() { nameof(Name), nameof(Description), nameof(Location), nameof(MaxCapacity) };
        }

        public bool HasEnoughSpace()
        {
            return this.CurrentCapacity <= this.MaxCapacity;
        }


        protected bool Equals(Warehouse other)
        {
            return Name == other.Name && Description == other.Description && Location == other.Location && MaxCapacity == other.MaxCapacity;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Warehouse)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Location != null ? Location.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ MaxCapacity.GetHashCode();
                return hashCode;
            }
        }

        public bool TryUpdate(Warehouse warehouse, bool changeId=false)
        {
            if (changeId)
            {
                this.Id = warehouse.Id;
            }

            if (this.Equals(warehouse))
            {
                return false;
            }

            this.Name = warehouse.Name;
            this.Description = warehouse.Description;
            this.Location = warehouse.Location;
            this.MaxCapacity = warehouse.MaxCapacity;

            return true;

        }
    }
}