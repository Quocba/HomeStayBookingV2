using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class RejectPostDTO
    {
        public Guid PostID { get; set; }

        public string ReasonReject { get; set; }
    }
}
