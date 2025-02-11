using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class ResetPasswordDTO
    {
        public Guid UserId { get; set; }
        public string Password { get; set; }
        public string Code { get; set; }
    }
}
