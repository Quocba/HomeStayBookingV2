public class PostDetailDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public DateTime PublishDate { get; set; }
    public Guid UserID { get; set; }
    public List<string> Images { get; set; }
    public List<CommentResponseDTO> Comments { get; set; }
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
