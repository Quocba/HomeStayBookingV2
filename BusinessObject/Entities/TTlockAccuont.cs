using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    [Table("TTLockAccount")]
    public class TTlockAccuont
    {
        [Key]
        public Guid TTLockID { get; set; }

        public string? TTLockUserName { get; set; }
        public string? Password { get; set; }

        public Guid? HomeStayID { get; set; }

        [ForeignKey("HomeStayID")]
        public HomeStay? HomeStay { get; set; }
    }
}
