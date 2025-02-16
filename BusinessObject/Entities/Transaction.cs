using BusinessObject.Common;
using BusinessObject.Shares;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class Transaction
    {
        [Key]
        public long ID { get; set; }
        public Guid BookingID { get; set; }
        public Booking Booking { get; set; }

        public string PaymentLink { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateUtility.GetCurrentDateTime();
    }
}
