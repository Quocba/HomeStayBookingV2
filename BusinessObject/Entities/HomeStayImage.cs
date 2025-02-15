using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Common;

namespace BusinessObject.Entities
{
    [Table("HomeStayImage")]
    public class HomeStayImage : BaseEntity<Guid>
    {
        [MaxLength(255)]
        public string Image {  get; set; }
        public Guid HomeStayID { get; set; }

        [ForeignKey("HomeStayID")]
        public HomeStay HomeStay { get; set; }
        public bool isDeleted { get; set; } = false;
    }
}
