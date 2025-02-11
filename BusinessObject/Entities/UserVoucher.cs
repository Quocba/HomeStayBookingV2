using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    [Table("UserVoucher")]
    public class UserVoucher
    {
        public Guid UserID {  get; set; }
        public User user { get; set; }

        public Guid VoucherID {  get; set; }
        public Voucher voucher { get; set; }

        public bool isUsed {  get; set; }
    }
}
