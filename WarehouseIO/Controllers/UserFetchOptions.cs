using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseIO.Controllers
{
    public enum UserFetchOptions
    {
        Default,
        IncludeWarehouseStoredItems,
        IncludeTransfers,
        IncludeTransfersWithTransferItems,
        IncludeWarehouseStoredItemsAndTransfers
    }
}