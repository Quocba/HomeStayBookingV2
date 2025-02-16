using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class PaymentReturnDTO
    {
        public string Code { get; set; }
        public string Id { get; set; }
        public string Cancel { get; set; }
        public string Status { get; set; }
        public string OrderCode { get; set; }
    }
}
