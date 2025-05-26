using PostsAPI.Data;
using PostsAPI.Models;

namespace PostsAPI.Services
{
	public class PostService : IPostService
	{
		private readonly IPostRepository _repository;

		public PostService(IPostRepository repository)
		{
			_repository = repository;
		}

		public async Task<IEnumerable<Post>> GetAllPostsAsync()
		{
			return await _repository.GetAllAsync();
		}

		public async Task<Post?> GetPostByIdAsync(string id)
		{
			return await _repository.GetByIdAsync(id);
		}

		public async Task<Post> CreatePostAsync(PostInput postInput)
		{
			var post = new Post
			{
				Id = Guid.NewGuid().ToString(),
				Author = postInput.Author,
				Date = postInput.Date,
				Content = postInput.Content
			};

			return await _repository.CreateAsync(post);
		}

		public async Task<Post?> UpdatePostAsync(string id, PostInput postInput)
		{
			var existingPost = await _repository.GetByIdAsync(id);
			if (existingPost == null)
			{
				return null;
			}

			existingPost.Author = postInput.Author;
			existingPost.Date = postInput.Date;
			existingPost.Content = postInput.Content;

			return await _repository.UpdateAsync(existingPost);
		}

		public async Task<bool> DeletePostAsync(string id)
		{
			return await _repository.DeleteAsync(id);
		}
	}
}