using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using WarehouseIO.ControlClasses;
using WarehouseIO.Exceptions;

namespace WarehouseIO.Models
{
    public class Item
    {

        public const string DEFAULT_IMAGE_URL = "/Content/Images/default-item-picture.png"; 
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }


        private ItemType _type;

        [NotMapped]
        public ItemType Type
        {
            get => _type; 
            set => _type = value;
        }

        [MaxLength(50)]
        public string TypeString
        {
            get => _type.ToString();
            set => _type = EnumHandler.GetValue<ItemType>(enumString: value);
        }

        public int Size { get; set; }

        [Display(Name = "Est. Price")]
        public double EstPrice { get; set; }

        public int Amount { get; set; }

        public int? WarehouseId { get; set; }

        public virtual Warehouse? Warehouse { get; set; }

        public string? ImageUrl { get; set; }

        [NotMapped]
        public string ImageUrlRender => this.ImageUrl ?? DEFAULT_IMAGE_URL;

        [NotMapped]
        public long SpaceTaken => (long) Math.Ceiling(this.Size * this.Amount / 1000.0);

        public Item()
        {
        }

        public long InTransferSpaceTaken(int amount)
        {
            return (long)Math.Ceiling(this.Size * amount / 1000.0);
        }

        public bool TryUpdate(Item item)
        {

            if (this.Equals(item))
            {
                return false;
            }

            if (this.Amount > item.Amount)
            {
                throw new InvalidAmountValueException("The new amount value is less then the current one. The new amount value can only be higher then the current one.");
            }

            this.Id = item.Id;
            this.Name = item.Name;
            this.Description = item.Description;
            this.Type = item.Type;
            this.Size = item.Size;
            this.EstPrice = item.EstPrice;  
            this.Amount = item.Amount;
            this.Warehouse = item.Warehouse;
            this.WarehouseId = item.WarehouseId;
            this.ImageUrl = item.ImageUrl;

            return true;
        }

        protected bool Equals(Item other)
        {
            return Id == other.Id && Name == other.Name && Description == other.Description && Size == other.Size &&
                   EstPrice.Equals(other.EstPrice) && Amount == other.Amount && WarehouseId == other.WarehouseId &&
                   ImageUrl == other.ImageUrl && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Item)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Size;
                hashCode = (hashCode * 397) ^ EstPrice.GetHashCode();
                hashCode = (hashCode * 397) ^ Amount;
                hashCode = (int)((hashCode * 397) ^ WarehouseId);
                hashCode = (hashCode * 397) ^ (ImageUrl != null ? ImageUrl.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}