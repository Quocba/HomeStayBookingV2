using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Common;
using BusinessObject.Shares;

namespace BusinessObject.Entities
{
    [Table("Booking")]
    public class Booking : BaseEntity<Guid>
    {
        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        public Decimal TotalPrice {  get; set; }

        public Decimal UnitPrice {  get; set; }

        public String Status {  get; set; }

        public String? ReasonCancel {  get; set; }
        public bool isDeleted { get; set; } = false;

        [MaxLength(255)]
        public string? HomeStayName { get; set; }

        [MaxLength(255)]
        public string? HomeStayAddress { get; set; }

        [MaxLength(255)]
        public string? HomeStayImage {  get; set; }

        public Guid UserID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }     
        public ICollection<Calendar> Calendars { get; set; }

    }
}
