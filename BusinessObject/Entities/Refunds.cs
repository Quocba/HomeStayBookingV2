using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    [Table("Refunds")]
    public class Refunds
    {
        [Key]
        public long RefundID { get; set; }

        public bool Status {  get; set; }

        public long TransactionID {  get; set; }

        [ForeignKey("TransactionID")]
        public Transaction Transaction { get; set; }

    }
}
