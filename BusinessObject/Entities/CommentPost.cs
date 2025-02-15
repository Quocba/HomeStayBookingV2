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
    [Table("CommentPost")]
    public class CommentPost : BaseEntity<Guid>
    {

        [MaxLength(255)]
        public string Comment {  get; set; }

        public DateTime CommontDate { get; set; } = DateUtility.GetCurrentDateTime();
        public Guid PostID { get; set; }
        [ForeignKey("PostID")]
        public Post Post { get; set; }
        public Guid UserID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }
        public Guid? ParrentID { get; set; }

        [ForeignKey("ParrentID")]
        public CommentPost? ReplyToUser { get; set; }
        public bool isDeleted { get; set; } = false;

        public ICollection<CommentPost>? ChildComments { get; set; }
    }
}
