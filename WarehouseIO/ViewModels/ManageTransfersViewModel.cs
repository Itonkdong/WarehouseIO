using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WarehouseIO.Models;

namespace WarehouseIO.ViewModels
{
    public class ManageTransfersViewModel
    {
        public string? ErrorMessage { get; set; }

        public List<Transfer> AllTransfers { get; set; } = new List<Transfer>();
        public List<Transfer> TransfersToAcceptOrReject { get; set; } = new List<Transfer>();

        public ManageTransfersViewModel()
        {
            
        }

    }
}