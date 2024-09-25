using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WarehouseIO.Models;

namespace WarehouseIO.ViewModels
{
    public class MakeShipmentViewModel : AViewModel
    {
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }

        [Display(Name = "Shipping To")]
        [Required]
        public string ShippingTo { get; set; }

        public List<MovingItemViewModel> AllWarehouseItem { get; set; } = new List<MovingItemViewModel>();

        public List<Warehouse> AllWarehouses { get; set; }

        public Shipment Shipment { get; set; }  


        public MakeShipmentViewModel()
        {

        }

    }

}