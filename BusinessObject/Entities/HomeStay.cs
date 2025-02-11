using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Common;

namespace BusinessObject.Entities
{
    [Table("HomeStay")]
    public class HomeStay : BaseEntity<Guid>
    {
        [MaxLength(255)]
        public string MainImage {  get; set; }

        [MaxLength(255)]
        public string Name {  get; set; }

        public int OpenIn {  get; set; }

        [MaxLength(255)]
        public string Description {  get; set; }

        public int Standar {  get; set; }

        public bool isDeleted {  get; set; }

        [MaxLength(255)]
        public string Address {  get; set; }

        [MaxLength(255)]
        public string City { get; set; }

        public bool isBooked {  get; set; }

        [MaxLength(255)]
        public string CheckInTime {  get; set; }

        [MaxLength(255)]
        public string CheckOutTime { get; set; }
    }
}
