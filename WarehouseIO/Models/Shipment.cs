using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WarehouseIO.ControlClasses;
using WarehouseIO.ViewModels;

namespace WarehouseIO.Models
{
    public class Shipment
    {
        //In minutes
        private const double TIME_BEFORE_CANCEL = 10;
        public int Id { get; set; }

        public int FromWarehouseId { get; set; }

        public virtual Warehouse FromWarehouse { get; set; }

        [MaxLength(200)]
        [Display(Name = "Shipping To")]
        public string ShippingTo { get; set; }

        [ForeignKey("ShipmentId")] 
        public ICollection<MovingItem> ShippingItems { get; set; } = new List<MovingItem>();

        public DateTime MadeOn { get; set; }

        [Display(Name = "Finalized On")]
        public DateTime? FinalizedOn { get; set; }

        private ShipmentStatus _status;

        [NotMapped]
        public ShipmentStatus Status
        {
            get => _status;
            set => _status = value;
        }

        [MaxLength(50)]
        public string ShipmentStatusString
        {
            get => _status.ToString();
            set => _status = EnumHandler.GetValue<ShipmentStatus>(value);
        }

        public Shipment()
        {
        }

        public MovingItem? GetItem(MovingItemViewModel movingItemVm)
        {
            return this
                .ShippingItems
                .FirstOrDefault(movingItem => movingItem.ItemId == movingItemVm.Id);

        }

        public bool CanBeCanceled()
        {
            return this.MadeOn - DateTime.Now <= TimeSpan.FromMinutes(TIME_BEFORE_CANCEL);
        }

        public TryResult TryCommit(Warehouse warehouse)
        {
            
            foreach (MovingItem movingItem in this.ShippingItems)
            {
                Item storedItem = warehouse.StoredItems
                    .FirstOrDefault(i => i.Id == movingItem.ItemId);

                 if (storedItem is null)
                 {
                     return new TryResult(false,
                         new Exception($"Item with ID: {movingItem.ItemId} is missing in the warehouse."));
                 }

                 if (storedItem.Amount < movingItem.Amount)
                 {
                     return new TryResult(false,
                         new Exception($"Item : {storedItem.Name} has insufficient shipping amount."));
                 }

                 storedItem!.Amount -= movingItem.Amount;

            }

            return new TryResult(true, null);
        }

        public TryResult TryCancel(ApplicationDbContext? db = null)
        {
            db ??= new ApplicationDbContext();
            try
            {

                foreach (MovingItem movingItem in this.ShippingItems)
                {
                    if (movingItem.Item.WarehouseId != null)
                    {
                        movingItem.Item.Amount += movingItem.Amount;
                    }
                    else
                    {
                        movingItem.Item.Amount = movingItem.Amount;
                        movingItem.Item.WarehouseId = this.FromWarehouseId;
                    }
                }

                db
                     .Shipments
                    .Remove(this);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                return new TryResult(false, e);
            }

            return new TryResult(true, null);
        }

        public void Finalize(Warehouse warehouse)
        {
            this.FinalizedOn = DateTime.Now;
            this.Status = ShipmentStatus.Finalized;
        }
    }
}