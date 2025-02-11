using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class ConfirmEmailDTO
    {
        public Guid UserId { get; set; }
        public string Code { get; set; }
    }
}
