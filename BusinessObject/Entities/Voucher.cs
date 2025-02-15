using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Common;
using BusinessObject.Shares;

namespace BusinessObject.Entities
{
    [Table("Voucher")]
    public class Voucher : BaseEntity<Guid>
    {

        [MaxLength(255)]
        public string Image {  get; set; }

        [MaxLength(6)]
        public string Code {  get; set; }

        public double Discount {  get; set; }

        [MaxLength(255)]
        public string Description {  get; set; }

        public int QuantityUsed {  get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool isDeleted {  get; set; } = false;
        public ICollection<UserVoucher> UserVouchers { get; set; }
    }
}
