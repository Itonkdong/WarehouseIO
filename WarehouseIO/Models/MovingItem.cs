/*#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WarehouseIO.Models
{
    public class MovingItem
    {
        public int Id { get; set; }

        [ForeignKey("Item")]
        public int ItemId { get; set; }

        public virtual Item Item { get; set; }

        [ForeignKey("Transfer")]
        public int? TransferId { get; set; }

        public virtual Transfer? Transfer { get; set; }

        [ForeignKey("Shipment")]
        public int? ShipmentId { get; set; }

        public virtual Shipment? Shipment { get; set; }

        public int Amount  { get; set; }

        public MovingItem()
        {
                
        }
    }
}*/