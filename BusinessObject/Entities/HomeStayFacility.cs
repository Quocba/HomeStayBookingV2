using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Common;

namespace BusinessObject.Entities
{
    [Table("HomeStayFacility")]
    public class HomeStayFacility
    {
        public Guid HomeStayID { get; set; }
        [ForeignKey("HomeStayID")]
        public HomeStay HomeStay { get; set; }

        public Guid FacilityID { get; set; }
        
        [ForeignKey("FacilityID")]
        public Facility Facility { get; set; }

        public int Quantity {  get; set; }
    }
}
