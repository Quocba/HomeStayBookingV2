using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class CalendarDTO
    {
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public Guid HomeStayID { get; set; }
    }

}
