using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class FeedbackDTO
    {
        public Guid HomestayID { get; set; }
        public int Rating { get; set; }
        public string Description { get; set; }
    }
}
