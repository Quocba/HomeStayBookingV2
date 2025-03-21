using System.Linq.Expressions;
using API.Controllers;
using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MockQueryable;
using Moq;
using Newtonsoft.Json;
#pragma warning disable

namespace APITesting;

public class PostControllerTesting
{
    private Mock<IRepository<Post>> _mockPostRepo;
    private Mock<IRepository<PostImage>> _mockPostImageRepo;
    private Mock<IRepository<CommentPost>> _mockCommentRepo;
    private PostController _controller;

    [SetUp]
    public void Setup()
    {
        _mockPostRepo = new Mock<IRepository<Post>>();
        _mockPostImageRepo = new Mock<IRepository<PostImage>>();
        _mockCommentRepo = new Mock<IRepository<CommentPost>>();

        _controller = new PostController(
            _mockPostRepo.Object,
            _mockPostImageRepo.Object,
            _mockCommentRepo.Object
        );
    }

        [Test]
        public async Task GetPosts_ReturnsOk_WithListOfPosts()
        {
            // Arrange
            var posts = new List<Post>
            {
                new Post
                {
                    Id = Guid.NewGuid(),
                    Title = "Post 1",
                    Description = "Description 1",
                    Location = "Location 1",
                    PublishDate = DateTime.UtcNow,
                    UserID = Guid.NewGuid(),
                    isDeleted = false,
                    PostImages = new List<PostImage>
                    {
                        new PostImage { Image = "image1.jpg" },
                        new PostImage { Image = "image2.jpg" }
                    }
                }
            };

            var mock = posts.AsQueryable().BuildMock();
            _mockPostRepo.Setup(r => r.Find(It.IsAny<Expression<Func<Post, bool>>>()))
                         .Returns(mock);

            // Act
            var result = await _controller.GetPosts();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }


    [Test]
    public async Task GetPosts_ReturnsNotFound_WhenNoPostsExist()
    {
        // Arrange
        var emptyPosts = new List<Post>().AsQueryable().BuildMock(); // Sử dụng MockQueryable.Moq

        var mockPostRepo = new Mock<IRepository<Post>>();
        var mockImageRepo = new Mock<IRepository<PostImage>>();
        var mockCommentRepo = new Mock<IRepository<CommentPost>>();

        mockPostRepo.Setup(r => r.Find(It.IsAny<Expression<Func<Post, bool>>>())).Returns(emptyPosts);

        var controller = new PostController(mockPostRepo.Object, mockImageRepo.Object, mockCommentRepo.Object);

        // Act
        var result = await controller.GetPosts();

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
        Assert.That(notFoundResult.Value.ToString(), Does.Contain("No posts found"));
    }

    [Test]
    public async Task GetPostDetail_ReturnsOk_WhenPostExists()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var commentId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            FullName = "Test User"
        };

        var comment = new CommentPost
        {
            Id = commentId,
            Comment = "Nice post",
            CommontDate = DateTime.Now,
            UserID = userId,
            User = user,
            PostID = postId,
            isDeleted = false
        };

        var reply = new CommentPost
        {
            Id = Guid.NewGuid(),
            Comment = "Thanks!",
            CommontDate = DateTime.Now.AddMinutes(1),
            UserID = userId,
            User = user,
            PostID = postId,
            ParrentID = commentId,
            isDeleted = false
        };

        var post = new Post
        {
            Id = postId,
            Title = "Post4 ",
            Description = "Post4 Description",
            Location = "Ho Chi Minh",
            PublishDate = DateTime.Now,
            UserID = userId,
            isDeleted = false,
            PostImages = new List<PostImage>
        {
            new PostImage { Image = "https://homestaybooking-001-site1.ntempurl.com/images/1dbf2c51-8bbc-4561-9aed-51e8e87596d9_4.jpg" }
        },
            CommentPosts = new List<CommentPost> { comment, reply }
        };

        var postQueryable = new List<Post> { post }.AsQueryable().BuildMock();

        _mockPostRepo.Setup(r => r.Find(It.IsAny<Expression<Func<Post, bool>>>()))
                     .Returns(postQueryable);

        var controller = new PostController(_mockPostRepo.Object, null, null);

        // Act
        var result = await controller.GetPostDetail(postId) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var json = JsonConvert.SerializeObject(result.Value);
        var postDetail = JsonConvert.DeserializeObject<PostDetailDTO>(json);

        Assert.NotNull(postDetail);
        Assert.AreEqual(postId, postDetail.Id);
        Assert.AreEqual("Post4 ", postDetail.Title);
        Assert.AreEqual("Post4 Description", postDetail.Description);
    }


    [Test]
    public async Task CreatePost_ReturnsOk_WhenPostIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var createDto = new CreatePostDTO
        {
            Title = "Test Title",
            Description = "Test Description",
            Location = "Hanoi",
            Images = new List<string> { "img1.jpg", "img2.jpg" }
        };

        _mockPostRepo.Setup(r => r.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        _mockPostRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CreatePost(createDto, userId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [Test]
    public async Task CreatePost_ReturnsBadRequest_WhenPostDTOIsNull()
    {
        // Arrange
        CreatePostDTO postDTO = null;
        var userId = Guid.NewGuid();

        // Act
        var result = await _controller.CreatePost(postDTO, userId);

        // Assert
        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
        Assert.That(badRequest.Value.ToString(), Contains.Substring("Invalid post data"));
    }

    [Test]
    public async Task EditPost_ReturnsOk_WhenPostExists()
    {
        // Arrange
        var postId = Guid.NewGuid();

        var existingPost = new Post
        {
            Id = postId,
            Title = "Old Title",
            Description = "Old Description",
            Location = "Old Location",
            PostImages = new List<PostImage>
        {
            new PostImage { Id = Guid.NewGuid(), Image = "old.jpg", PostID = postId }
        }
        };

        var updatedPostDto = new EditPostDTO
        {
            Title = "New Title",
            Description = "New Description",
            Location = "New Location",
            Images = new List<string> { "new1.jpg", "new2.jpg" }
        };

        var postQueryable = new List<Post> { existingPost }.AsQueryable().BuildMock();

        _mockPostRepo.Setup(r => r.Find(It.IsAny<Expression<Func<Post, bool>>>()))
                     .Returns(postQueryable);

        _mockPostImageRepo.Setup(r => r.DeleteRange(It.IsAny<IEnumerable<PostImage>>()));
        _mockPostImageRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        _mockPostImageRepo.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<PostImage>>()))
                          .Returns(Task.CompletedTask);

        _mockPostRepo.Setup(r => r.UpdateAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        _mockPostRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var controller = new PostController(_mockPostRepo.Object, _mockPostImageRepo.Object, _mockCommentRepo.Object);

        // Act
        var result = await controller.EditPost(postId, updatedPostDto);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.That(okResult.Value.ToString(), Does.Contain("Post updated successfully"));
    }

    [Test]
    public async Task EditPost_ReturnsNotFound_WhenPostDoesNotExist()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var updatedPostDto = new EditPostDTO
        {
            Title = "New Title",
            Description = "New Description",
            Location = "New Location",
            Images = new List<string> { "new1.jpg", "new2.jpg" }
        };

        var emptyList = new List<Post>().AsQueryable();

        _mockPostRepo.Setup(r => r.Find(It.IsAny<Expression<Func<Post, bool>>>()))
                     .Returns(emptyList.BuildMock());

        var controller = new PostController(_mockPostRepo.Object, _mockPostImageRepo.Object, _mockCommentRepo.Object);

        // Act
        var result = await controller.EditPost(postId, updatedPostDto);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
        Assert.That(notFoundResult.Value.ToString(), Does.Contain("Post not found"));
    }

    [Test]
    public async Task DeletePost_ReturnsOk_WhenPostExists()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var existingPost = new Post
        {
            Id = postId,
            isDeleted = false
        };

        var postQueryable = new List<Post> { existingPost }.AsQueryable().BuildMock();

        _mockPostRepo.Setup(r => r.Find(It.IsAny<Expression<Func<Post, bool>>>()))
                     .Returns(postQueryable);

        _mockPostRepo.Setup(r => r.UpdateAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
        _mockPostRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var controller = new PostController(_mockPostRepo.Object, _mockPostImageRepo.Object, _mockCommentRepo.Object);

        // Act
        var result = await controller.DeletePost(postId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.That(okResult.Value.ToString(), Does.Contain("Post deleted successfully"));

        Assert.IsTrue(existingPost.isDeleted);
    }

    [Test]
    public async Task DeletePost_ReturnsNotFound_WhenPostDoesNotExist()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var emptyList = new List<Post>().AsQueryable();

        _mockPostRepo.Setup(r => r.Find(It.IsAny<Expression<Func<Post, bool>>>()))
                     .Returns(emptyList.BuildMock());

        var controller = new PostController(_mockPostRepo.Object, _mockPostImageRepo.Object, _mockCommentRepo.Object);

        // Act
        var result = await controller.DeletePost(postId);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
        Assert.That(notFoundResult.Value.ToString(), Does.Contain("Post not found"));
    }

    [Test]
    public async Task CreateComment_ReturnsOk_WhenValidComment()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var postId = Guid.NewGuid();
        var parentId = Guid.NewGuid();

        var commentDTO = new CreateCommentDTO
        {
            Comment = "This is a test comment",
            PostID = postId,
            ParentID = parentId
        };

        _mockCommentRepo.Setup(r => r.AddAsync(It.IsAny<CommentPost>())).Returns(Task.CompletedTask);
        _mockCommentRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var controller = new PostController(_mockPostRepo.Object, _mockPostImageRepo.Object, _mockCommentRepo.Object);

        // Act
        var result = await controller.CreateComment(userId, commentDTO);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.That(okResult.Value.ToString(), Does.Contain("Comment created successfully"));

        _mockCommentRepo.Verify(r => r.AddAsync(It.IsAny<CommentPost>()), Times.Once);
        _mockCommentRepo.Verify(r => r.SaveAsync(), Times.Once);
    }

    [Test]
    public async Task CreateComment_ReturnsBadRequest_WhenCommentIsEmpty()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var commentDTO = new CreateCommentDTO
        {
            Comment = "",
            PostID = Guid.NewGuid(),
            ParentID = null
        };

        var controller = new PostController(_mockPostRepo.Object, _mockPostImageRepo.Object, _mockCommentRepo.Object);

        // Act
        var result = await controller.CreateComment(userId, commentDTO);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.That(badRequestResult.Value.ToString(), Does.Contain("Comment cannot be empty"));
    }

    [Test]
    public async Task EditComment_ReturnsOk_WhenCommentIsUpdated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var commentId = Guid.NewGuid();

        var existingComment = new CommentPost
        {
            Id = commentId,
            Comment = "Old Comment",
            UserID = userId
        };

        var commentList = new List<CommentPost> { existingComment }.AsQueryable().BuildMock();
        _mockCommentRepo.Setup(r => r.Find(It.IsAny<Expression<Func<CommentPost, bool>>>()))
                        .Returns(commentList);

        _mockCommentRepo.Setup(r => r.UpdateAsync(It.IsAny<CommentPost>())).Returns(Task.CompletedTask);
        _mockCommentRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var controller = new PostController(_mockPostRepo.Object, _mockPostImageRepo.Object, _mockCommentRepo.Object);

        var commentDTO = new EditCommentDTO
        {
            Comment = "Updated Comment"
        };

        // Act
        var result = await controller.EditComment(userId, commentId, commentDTO);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.That(okResult.Value.ToString(), Does.Contain("Comment updated successfully"));
    }

    [Test]
    public async Task EditComment_ReturnsNotFound_WhenCommentDoesNotExist()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var emptyList = new List<CommentPost>().AsQueryable().BuildMock();

        _mockCommentRepo.Setup(r => r.Find(It.IsAny<Expression<Func<CommentPost, bool>>>()))
                        .Returns(emptyList);

        var controller = new PostController(_mockPostRepo.Object, _mockPostImageRepo.Object, _mockCommentRepo.Object);

        var commentDTO = new EditCommentDTO
        {
            Comment = "Attempted Edit"
        };

        // Act
        var result = await controller.EditComment(userId, commentId, commentDTO);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
        Assert.That(notFoundResult.Value.ToString(), Does.Contain("Comment not found"));
    }

    [Test]
    public async Task EditComment_ReturnsForbid_WhenUserNotOwner()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var ownerId = Guid.NewGuid(); // Người tạo comment
        var otherUserId = Guid.NewGuid(); // Người đang gửi request

        var existingComment = new CommentPost
        {
            Id = commentId,
            Comment = "Original Comment",
            UserID = ownerId
        };

        var commentList = new List<CommentPost> { existingComment }.AsQueryable().BuildMock();

        _mockCommentRepo.Setup(r => r.Find(It.IsAny<Expression<Func<CommentPost, bool>>>()))
                        .Returns(commentList);

        var controller = new PostController(_mockPostRepo.Object, _mockPostImageRepo.Object, _mockCommentRepo.Object);

        var commentDTO = new EditCommentDTO
        {
            Comment = "Hacked Edit"
        };

        // Act
        var result = await controller.EditComment(otherUserId, commentId, commentDTO);

        // Assert
        Assert.IsInstanceOf<ForbidResult>(result);
    }

    [Test]
    public async Task DeleteComment_ReturnsOk_WhenCommentDeletedByOwner()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var existingComment = new CommentPost
        {
            Id = commentId,
            UserID = userId,
            isDeleted = false
        };

        var mockData = new List<CommentPost> { existingComment }.AsQueryable().BuildMock();

        _mockCommentRepo.Setup(r => r.Find(It.IsAny<Expression<Func<CommentPost, bool>>>()))
                        .Returns(mockData);
        _mockCommentRepo.Setup(r => r.UpdateAsync(It.IsAny<CommentPost>())).Returns(Task.CompletedTask);
        _mockCommentRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var controller = new PostController(_mockPostRepo.Object, _mockPostImageRepo.Object, _mockCommentRepo.Object);

        // Act
        var result = await controller.DeleteComment(userId, commentId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.That(okResult.Value.ToString(), Does.Contain("Comment deleted successfully"));
    }

    [Test]
    public async Task DeleteComment_ReturnsNotFound_WhenCommentDoesNotExist()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var emptyList = new List<CommentPost>().AsQueryable().BuildMock();

        _mockCommentRepo.Setup(r => r.Find(It.IsAny<Expression<Func<CommentPost, bool>>>()))
                        .Returns(emptyList);

        var controller = new PostController(_mockPostRepo.Object, _mockPostImageRepo.Object, _mockCommentRepo.Object);

        // Act
        var result = await controller.DeleteComment(userId, commentId);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
        Assert.That(notFoundResult.Value.ToString(), Does.Contain("Comment not found"));
    }

    [Test]
    public async Task DeleteComment_ReturnsForbid_WhenUserNotOwner()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var attackerId = Guid.NewGuid(); // Không phải chủ comment

        var existingComment = new CommentPost
        {
            Id = commentId,
            UserID = ownerId
        };

        var commentList = new List<CommentPost> { existingComment }.AsQueryable().BuildMock();

        _mockCommentRepo.Setup(r => r.Find(It.IsAny<Expression<Func<CommentPost, bool>>>()))
                        .Returns(commentList);

        var controller = new PostController(_mockPostRepo.Object, _mockPostImageRepo.Object, _mockCommentRepo.Object);

        // Act
        var result = await controller.DeleteComment(attackerId, commentId);

        // Assert
        Assert.IsInstanceOf<ForbidResult>(result);
    }


}
