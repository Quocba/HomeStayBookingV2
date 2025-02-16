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
    [Table("Post")]
    public class Post : BaseEntity<Guid>
    {
        [MaxLength(255)]
        public string Title { get; set; }
        
        [MaxLength(255)]
        public string Description { get; set; }

        [MaxLength(255)]
        public string Location {  get; set; }

        [MaxLength(255)]
        public string Status {  get; set; } = "Pending";

        [MaxLength(255)]
        public string ReasonReject {  get; set; } = "No";

        public DateTime PublishDate { get; set; } = DateUtility.GetCurrentDateTime();
        public bool isDeleted { get; set; } = false;
        public Guid UserID { get; set; }
        [ForeignKey("UserID")]
        public User User { get; set; }
        public ICollection<PostImage> PostImages { get; set; }
        public ICollection<CommentPost> CommentPosts { get; set; }
    }
}
