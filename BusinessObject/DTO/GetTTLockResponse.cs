using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class GetTTLockResponse
    {
        public int lockId {  get; set; }

        public string lockAlias {  get; set; }

        public int electricQuantity {  get; set; }
    }
}
