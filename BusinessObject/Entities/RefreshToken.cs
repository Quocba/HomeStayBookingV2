using BusinessObject.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class RefreshToken : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        [StringLength(250)]
        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }
    }

}
