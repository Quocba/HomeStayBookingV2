using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class PostDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime PublishDate { get; set; }
        public Guid UserID { get; set; }
        public List<string> Images { get; set; }
        public List<CommentDTO> Comments { get; set; }
    }

    public class CreatePostDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public List<string> Images { get; set; }
    }

    public class EditPostDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public List<string> Images { get; set; }
    }

    public class CommentDTO
    {
        public Guid Id { get; set; }
        public string Comment { get; set; }
        public DateTime CommontDate { get; set; }
        public Guid? ParrentID { get; set; }
    }

    public class CreateCommentDTO
    {
        public string Comment { get; set; }
        public Guid PostID { get; set; }
        public Guid? ParentID { get; set; }
    }

    public class EditCommentDTO
    {
        public string Comment { get; set; }
    }

    public class CommentResponseDTO
    {
        public Guid Id { get; set; }
        public string Comment { get; set; }
        public DateTime CommentDate { get; set; }
        public Guid UserID { get; set; }
        public string FullName { get; set; }
        public Guid PostID { get; set; }
        public List<CommentResponseDTO> Replies { get; set; }
    }


}
