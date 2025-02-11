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

        [ForeignKey("PostID")]
        public Post Post { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        [ForeignKey("ParrentID")]
        public User? ReplyToUser { get; set; }
        public bool isDeleted { get; set; } = false;


    }
}
