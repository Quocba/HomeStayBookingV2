using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTO;

namespace APITesting.DTO
{
     public class HomeStayDTO
    {
        public string Name { get; set; }
        public string City { get; set; }
        public IEnumerable<CalendarDTO> Calendar { get; set; }
        public IEnumerable<AddAmenityDTO> Amenities { get; set; }
        public IEnumerable<AddFacilityDTO> Facility { get; set; }
    }
}
