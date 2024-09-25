using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WarehouseIO.Models;

namespace WarehouseIO.ViewModels
{
    public class DashboardViewModel : AViewModel
    {
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }

        public List<Warehouse> AllWarehouses { get; set; } = new List<Warehouse>();

        public DashboardViewModel()
        {
        }
    }
}