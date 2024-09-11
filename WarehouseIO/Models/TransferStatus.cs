using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WarehouseIO.Models
{
    public enum TransferStatus
    {
        [Display(Name = "Still Pending")]
        StillPending,
        Accepted,
        Rejected

    }
}