using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WarehouseIO.Models;

namespace WarehouseIO.ViewModels
{
    public class ManageOperatorsViewModel : AViewModel
    {
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }

        public List<Warehouse> AllWarehouses { get; set; } = new List<Warehouse>();

        [Display(Name = "Operator's Email")]
        public string NewOperatorEmail { get; set; }

        public ManageOperatorsViewModel()
        {
                
        }

        public bool IsUserManagerInWarehouse(string userEmail)
        {
            return this.Warehouse
                .Managers
                .Any(m => m.Email == userEmail);
        }

        public string GetUsersRoles(string userEmail)
        {
            return this.Warehouse.Managers.Any(m => m.Email == userEmail) switch
            {
                true => "Manager",
                false => "Operator"
            };
        }


    }
}