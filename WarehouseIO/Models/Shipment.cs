using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WarehouseIO.Models
{
    public class Shipment
    {
        public int Id { get; set; }

        public int FromWarehouseId { get; set; }

        public virtual Warehouse FromWarehouse { get; set; }

        [MaxLength(200)]
        public string ShippingTo { get; set; }

        [ForeignKey("ShipmentId")]
        public ICollection<MovingItem> ShippingItems { get; set; }

        public DateTime MadeOn { get; set; }

        public Shipment()
        {
        }
    }
}