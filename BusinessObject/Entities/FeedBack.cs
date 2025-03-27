using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BusinessObject.Common;

namespace BusinessObject.Entities
{
    [Table("FeedBack")]
    public class FeedBack : BaseEntity<Guid>
    {
        public int Rating {  get; set; }
        public string Description {  get; set; }
        public bool IsReply { get; set; } = false;

        public Guid UserID { get; set; }
        [ForeignKey("UserID")]
        public User User { get; set; }
        [JsonIgnore]
        [ForeignKey("HomeStayID")]
        public HomeStay HomeStay { get; set; }

    }
}
