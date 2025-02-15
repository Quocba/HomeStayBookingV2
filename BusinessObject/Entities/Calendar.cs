using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Common;

namespace BusinessObject.Entities
{
    [Table("Calendar")]
    public class Calendar : BaseEntity<Guid>
    {
        public DateTime Date { get; set; }
        public Decimal Price {  get; set; }
        public Guid HomeStayID { get; set; }

        [ForeignKey("HomeStayID")]
        public HomeStay HomeStay { get; set; }
        public Guid? BookingID { get; set; }

        [ForeignKey("BookingID")]
        public Booking? Booking { get; set; }

        public bool isDeleted {  get; set; } = false;
    }
}
