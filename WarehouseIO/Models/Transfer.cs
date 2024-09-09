/*using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WarehouseIO.Models
{
    public class Transfer
    {
        public int Id { get; set; }

        [ForeignKey("ApplicationUser")] 
        public int MadeByUserId { get; set; }

        public virtual ApplicationUser MadeByUser { get; set; }

        public int FromWarehouseId { get; set; }

        public virtual Warehouse FromWarehouse { get; set; }

        public int ToWarehouseId { get; set; }

        public virtual Warehouse ToWarehouse { get; set; }

        public DateTime MadeOn { get; set; }
        public DateTime? ClosedOn { get; set; }

        public TransferStatus Status { get; set; }

        public virtual ICollection<MovingItem> TransferItems { get; set; } = new List<MovingItem>();

        public Transfer()
        {
        }
    }
}*/