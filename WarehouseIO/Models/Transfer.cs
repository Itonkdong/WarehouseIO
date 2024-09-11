using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WarehouseIO.ControlClasses;

namespace WarehouseIO.Models
{
    public class Transfer
    {
        public int Id { get; set; }

        public string MadeByUserId { get; set; }

        public virtual ApplicationUser MadeByUser { get; set; }

        public int FromWarehouseId { get; set; }

        public virtual Warehouse FromWarehouse { get; set; }

        public int ToWarehouseId { get; set; }

        public virtual Warehouse ToWarehouse { get; set; }

        public DateTime MadeOn { get; set; }
        public DateTime? ClosedOn { get; set; }


        private TransferStatus _status;

        [NotMapped]
        public TransferStatus Status
        {
            get => _status;
            set => _status = value;
        }

        [MaxLength(50)]
        public string TransferStatusString
        {
            get => _status.ToString();
            set => EnumHandler.GetValue<TransferStatus>(value);
        }

        [ForeignKey("TransferId")]
        public virtual ICollection<MovingItem> TransferItems { get; set; } = new List<MovingItem>();

        [NotMapped]
        public double EstTransferValue =>
            TransferItems
                .Select(item => item.EstPrice * item.Amount)
                .Sum();

        public Transfer()
        {
        }

        protected bool Equals(Transfer other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Transfer)obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}