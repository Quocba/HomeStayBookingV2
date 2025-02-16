using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class BookingDTO
    {
        public List<BookingCalendarDTO> Calenders { get; set; }
        public string? VoucherCode { get; set; }
        public bool IsOnline { get; set; } // true: Online | false: Offline
    }

    public class BookingCalendarDTO
    {
        public Guid CalenderID { get; set; }
    }
}
