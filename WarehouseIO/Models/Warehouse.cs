/*using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseIO.Models
{
    public class Warehouse
    {
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string Description { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string Location { get; set; }

        public long MaxCapacity { get; set; }

        public virtual ICollection<Item> StoredItems { get; set; } = new List<Item>();
        public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
        public virtual ICollection<Transfer> TransfersFromWarehouse { get; set; } = new List<Transfer>();
        public virtual ICollection<Transfer> TransfersToWarehouse { get; set; } = new List<Transfer>();
        public virtual ICollection<ApplicationUser> Operators { get; set; } = new List<ApplicationUser>();

        public Warehouse()
        { 

            
        }
    }
}*/