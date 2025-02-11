using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    [Table("HomestayAmenity")]
    public class HomestayAmenity
    {
        public Guid AmenityID {  get; set; }
        public Amennity Amennity { get; set; }
        public Guid HomeStayID {  get; set; }
        public HomeStay HomeStay { get; set; }
        public bool isDeleted { get; set; } = false;
    }
}
