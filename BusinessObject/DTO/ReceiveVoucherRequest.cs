using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class ReceiveVoucherRequest
    {
        [Required]
        public Guid UserID { get; set; }

        [Required]
        public Guid VoucherID { get; set; }
    }

}
