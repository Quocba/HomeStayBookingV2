using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class CancelBookingDTO
    {
        public Guid BookingID { get; set; }
        public string ReasonCancel {  get; set; }
    }
}
