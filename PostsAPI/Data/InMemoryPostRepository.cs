using System.Collections.Concurrent;
using PostsAPI.Models;

namespace PostsAPI.Data
{
	public class InMemoryPostRepository : IPostRepository
	{
		private readonly ConcurrentDictionary<string, Post> _posts = new();

		public Task<IEnumerable<Post>> GetAllAsync()
		{
			return Task.FromResult(_posts.Values.AsEnumerable());
		}

		public Task<Post?> GetByIdAsync(string id)
		{
			_posts.TryGetValue(id, out var post);
			return Task.FromResult(post);
		}

		public Task<Post> CreateAsync(Post post)
		{
			_posts[post.Id] = post;
			return Task.FromResult(post);
		}

		public Task<Post> UpdateAsync(Post post)
		{
			_posts[post.Id] = post;
			return Task.FromResult(post);
		}

		public Task<bool> DeleteAsync(string id)
		{
			return Task.FromResult(_posts.TryRemove(id, out _));
		}
	}
}