using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class LockInfroDTO
    {
        public int LockID {  get; set; }

        public long LockPassword { get; set; }

        public string LockName { get; set; }
    }
}
