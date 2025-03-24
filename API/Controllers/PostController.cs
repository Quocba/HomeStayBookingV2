using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController(IRepository<Post> _postRepository, IRepository<PostImage> _postImageRepository,
        IRepository<CommentPost> _commentRepository) : ControllerBase
    {
        [HttpGet("list")]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _postRepository.Find(p => !p.isDeleted)
                .Select(p => new PostDTO
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Location = p.Location,
                    PublishDate = p.PublishDate,
                    UserID = p.UserID,
                    Images = p.PostImages.Select(i => i.Image).ToList()
                })
                .ToListAsync();

            if (!posts.Any())
                return NotFound(new { Message = "No posts found" });

            return Ok(posts);
        }
        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetPostDetail(Guid id)
        {
            var post = await _postRepository
                .Find(p => p.Id == id && !p.isDeleted)
                .Include(p => p.PostImages)
                .Include(p => p.CommentPosts)
                .ThenInclude(c => c.User)
                .Select(p => new
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Location = p.Location,
                    PublishDate = p.PublishDate,
                    UserID = p.UserID,
                    Images = p.PostImages.Select(i => i.Image).ToList(),
                    Comments = p.CommentPosts
                        .Where(c => !c.isDeleted && c.ParrentID == null)
                        .OrderByDescending(c => c.CommontDate)
                        .Select(c => new CommentResponseDTO
                        {
                            Id = c.Id,
                            Comment = c.Comment,
                            CommentDate = c.CommontDate,
                            UserID = c.UserID,
                            FullName = c.User.FullName,
                            Avatar = c.User.Avatar!,
                            PostID = c.PostID,
                            Replies = p.CommentPosts
                                .Where(r => r.ParrentID == c.Id && !r.isDeleted && r.ParrentID != null)
                                .OrderBy(r => r.CommontDate)
                                .Select(r => new CommentResponseDTO
                                {
                                    Id = r.Id,
                                    Comment = r.Comment,
                                    CommentDate = r.CommontDate,
                                    UserID = r.UserID,
                                    FullName = r.User.FullName,
                                    Avatar = r.User.Avatar!,
                                    PostID = r.PostID
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (post == null)
                return NotFound(new { Message = "Post not found" });

            return Ok(post);
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDTO postDTO, [FromHeader(Name = "X-User-Id")] Guid userId)
        {
            if (postDTO == null)
                return BadRequest(new { Message = "Invalid post data" });

            var ID = Guid.NewGuid();

            var newPost = new Post
            {
                Id = ID,
                Title = postDTO.Title,
                Description = postDTO.Description,
                Location = postDTO.Location,
                UserID = userId,
                PublishDate = DateTime.UtcNow,
                isDeleted = false,
                PostImages = postDTO.Images.Select(i => new PostImage
                {
                    Id = Guid.NewGuid(),
                    Image = i,
                    PostID = ID
                }).ToList()
            };

            await _postRepository.AddAsync(newPost);
            await _postRepository.SaveAsync();

            return Ok(new { Message = "Post created successfully", PostID = newPost.Id });
        }


        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditPost(Guid id, [FromBody] EditPostDTO updatedPost)
        {
            var existingPost = await _postRepository
                .Find(p => p.Id == id)
                .Include(p => p.PostImages)
                .FirstOrDefaultAsync();

            if (existingPost == null)
                return NotFound(new { Message = "Post not found" });

            existingPost.Title = updatedPost.Title ?? existingPost.Title;
            existingPost.Description = updatedPost.Description ?? existingPost.Description;
            existingPost.Location = updatedPost.Location ?? existingPost.Location;

            if (existingPost.PostImages.Any())
            {
                var imagesToDelete = existingPost.PostImages.ToList();
                _postImageRepository.DeleteRange(imagesToDelete);
                await _postImageRepository.SaveAsync();
            }

            if (updatedPost.Images != null && updatedPost.Images.Any())
            {
                var newImages = updatedPost.Images.Select(i => new PostImage
                {
                    Id = Guid.NewGuid(),
                    Image = i,
                    PostID = existingPost.Id
                }).ToList();

                await _postImageRepository.AddRangeAsync(newImages);
                await _postImageRepository.SaveAsync();
            }

            await _postRepository.UpdateAsync(existingPost);
            await _postRepository.SaveAsync();

            return Ok(new { Message = "Post updated successfully" });
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var post = await _postRepository.Find(p => p.Id == id).FirstOrDefaultAsync();

            if (post == null)
                return NotFound(new { Message = "Post not found" });

            post.isDeleted = true;
            await _postRepository.UpdateAsync(post);
            await _postRepository.SaveAsync();

            return Ok(new { Message = "Post deleted successfully" });
        }

        [HttpPost("create-comment")]
        public async Task<IActionResult> CreateComment(
        [FromHeader(Name = "X-User-Id")] Guid userId,
        [FromBody] CreateCommentDTO commentDTO)
        {
            if (string.IsNullOrWhiteSpace(commentDTO.Comment))
                return BadRequest(new { Message = "Comment cannot be empty" });

            var newComment = new CommentPost
            {
                Id = Guid.NewGuid(),
                Comment = commentDTO.Comment,
                CommontDate = DateTime.UtcNow,
                PostID = commentDTO.PostID,
                UserID = userId,
                ParrentID = commentDTO.ParentID
            };

            await _commentRepository.AddAsync(newComment);
            await _commentRepository.SaveAsync();

            return Ok(new { Message = "Comment created successfully", CommentID = newComment.Id });
        }

        [HttpPut("edit-comment/{id}")]
        public async Task<IActionResult> EditComment(
            [FromHeader(Name = "X-User-Id")] Guid userId,
            Guid id,
            [FromBody] EditCommentDTO commentDTO)
        {
            var existingComment = await _commentRepository.Find(c => c.Id == id).FirstOrDefaultAsync();

            if (existingComment == null)
                return NotFound(new { Message = "Comment not found" });

            if (existingComment.UserID != userId)
                return Forbid(); // Không cho sửa comment của người khác

            existingComment.Comment = commentDTO.Comment ?? existingComment.Comment;
            await _commentRepository.UpdateAsync(existingComment);
            await _commentRepository.SaveAsync();

            return Ok(new { Message = "Comment updated successfully" });
        }

        [HttpDelete("delete-comment/{id}")]
        public async Task<IActionResult> DeleteComment(
            [FromHeader(Name = "X-User-Id")] Guid userId,
            Guid id)
        {
            var comment = await _commentRepository.Find(c => c.Id == id).FirstOrDefaultAsync();

            if (comment == null)
                return NotFound(new { Message = "Comment not found" });

            if (comment.UserID != userId)
                return Forbid(); // Không cho xóa comment của người khác

            comment.isDeleted = true;
            await _commentRepository.UpdateAsync(comment);
            await _commentRepository.SaveAsync();

            return Ok(new { Message = "Comment deleted successfully" });
        }


    }
}
