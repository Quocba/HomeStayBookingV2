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
    [Table("PostImage")]
    public class PostImage : BaseEntity<Guid>
    {

        [MaxLength(255)]
        public string Image {  get; set; }

        [ForeignKey("PostID")]
        public Post Post { get; set; }
        public bool isDeleted { get; set; } = false;
    }
}
