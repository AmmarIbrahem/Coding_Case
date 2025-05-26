using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using PostsAPI.Models;
using PostsAPI.Services;

namespace PostsAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class PostsController : ControllerBase
	{
		private readonly IPostService _postService;
		private readonly ILogger<PostsController> _logger;

		public PostsController(IPostService postService, ILogger<PostsController> logger)
		{
			_postService = postService;
			_logger = logger;
		}

		/// <summary>
		/// List all posts
		/// </summary>
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Post>))]
		public async Task<ActionResult<IEnumerable<Post>>> GetAllPosts()
		{
			_logger.LogInformation("Getting all posts");
			var posts = await _postService.GetAllPostsAsync();
			return Ok(posts);
		}

		/// <summary>
		/// Get a post by ID
		/// </summary>
		[HttpGet("{postId}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Post))]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<Post>> GetPost(string postId)
		{
			_logger.LogInformation("Getting post with ID: {PostId}", postId);

			var post = await _postService.GetPostByIdAsync(postId);
			if (post == null)
			{
				_logger.LogWarning("Post not found with ID: {PostId}", postId);
				return NotFound();
			}

			return Ok(post);
		}

		/// <summary>
		/// Create a new post
		/// </summary>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Post))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<Post>> CreatePost([FromBody] PostInput postInput)
		{
			_logger.LogInformation("Creating new post by author: {Author}", postInput.Author);

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var createdPost = await _postService.CreatePostAsync(postInput);

			_logger.LogInformation("Created post with ID: {PostId}", createdPost.Id);
			return CreatedAtAction(nameof(GetPost), new { postId = createdPost.Id }, createdPost);
		}

		/// <summary>
		/// Update a post by ID
		/// </summary>
		[HttpPut("{postId}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Post))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<Post>> UpdatePost(string postId, [FromBody] PostInput postInput)
		{
			_logger.LogInformation("Updating post with ID: {PostId}", postId);

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var updatedPost = await _postService.UpdatePostAsync(postId, postInput);
			if (updatedPost == null)
			{
				_logger.LogWarning("Post not found for update with ID: {PostId}", postId);
				return NotFound();
			}

			_logger.LogInformation("Updated post with ID: {PostId}", postId);
			return Ok(updatedPost);
		}

		/// <summary>
		/// Delete a post by ID
		/// </summary>
		[HttpDelete("{postId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> DeletePost(string postId)
		{
			_logger.LogInformation("Deleting post with ID: {PostId}", postId);

			var deleted = await _postService.DeletePostAsync(postId);
			if (!deleted)
			{
				_logger.LogWarning("Post not found for deletion with ID: {PostId}", postId);
				return NotFound();
			}

			_logger.LogInformation("Deleted post with ID: {PostId}", postId);
			return NoContent();
		}
	}
}