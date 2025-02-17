using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class AddHomeStayFacilityDTO
    {
        public Guid HomeStayID { get; set; }
        public Guid FacilityID { get; set; }
        public int Quantity { get; set; }


    }
}
