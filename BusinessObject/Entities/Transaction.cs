using BusinessObject.Common;
using BusinessObject.Shares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class Transaction : BaseEntity<long>
    {
        public Guid BookingID { get; set; }
        public Booking Booking { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateUtility.GetCurrentDateTime();
    }
}
