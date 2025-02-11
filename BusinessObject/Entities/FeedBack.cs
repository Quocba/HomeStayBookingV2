using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Common;

namespace BusinessObject.Entities
{
    [Table("FeedBack")]
    public class FeedBack : BaseEntity<Guid>
    {
        public int Rating {  get; set; }
        public string Description {  get; set; }
        public bool isDeleted { get; set; } = false;

        [ForeignKey("UserID")]
        public User User { get; set; }

        [ForeignKey("HomeStayID")]
        public HomeStay HomeStay { get; set; }




    }
}
