using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class ChangeDeletedUserDTO
    {
        public Guid UserId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
