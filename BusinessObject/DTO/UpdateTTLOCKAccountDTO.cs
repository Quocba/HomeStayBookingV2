using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class UpdateTTLOCKAccountDTO
    {
        public string? TTLockUserName { get; set; }
        public string? Password { get; set; }

        public Guid? HomeStayID { get; set; }
    }
}
