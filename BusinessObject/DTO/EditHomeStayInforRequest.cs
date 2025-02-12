using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class EditHomeStayInforRequest
    {
        public  Guid HomeStayID { get; set; }
        public string MainImage { get; set; }
        public string Name { get; set; }
        public int OpenIn { get; set; }
        public string Description { get; set; }
        public int Standar { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string CheckInTime { get; set; }
        public string CheckOutTime { get; set; }

    }
}
