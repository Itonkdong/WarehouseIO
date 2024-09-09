/*using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace WarehouseIO.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string Description { get; set; }

        [EnumDataType(typeof(Item))]
        [Column(TypeName = "nvarchar(80)")]
        public ItemType Type { get; set; }

        public int Size { get; set; }

        [Display(Name = "Est. Price")]
        public double EstPrice { get; set; }

        public int Amount { get; set; }

        [ForeignKey("Warehouse")]
        public int WarehouseId { get; set; }
        public virtual Warehouse Warehouse { get; set; }

        public Item()
        {
        }
    }
}*/