#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WarehouseIO.ViewModels;

namespace WarehouseIO.Models
{
    public class MovingItem
    {


        public int Id { get; set; }

        [ForeignKey("Item")]
        public int ItemId { get; set; }

        public virtual Item Item { get; set; }

        public int? TransferId { get; set; }

        public virtual Transfer? Transfer { get; set; }

        public int? ShipmentId { get; set; }

        public virtual Shipment? Shipment { get; set; }

        public int Amount  { get; set; }

        public double EstPrice { get; set; }

        [NotMapped]
        public long SpaceTaken => (long)Math.Ceiling(this.Item.Size * this.Amount / 1000.0);



        public MovingItem()
        {
                
        }

        public MovingItem(MovingItemViewModel item)
        {
            this.ItemId = item.Id;
            this.Amount = item.TransferAmount;
            this.EstPrice = item.EstPrice;
        }
    }
}