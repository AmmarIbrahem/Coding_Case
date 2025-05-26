using PostsAPI.Models;

namespace PostsAPI.Services
{
	public interface IPostService
	{
		Task<IEnumerable<Post>> GetAllPostsAsync();
		Task<Post?> GetPostByIdAsync(string id);
		Task<Post> CreatePostAsync(PostInput postInput);
		Task<Post?> UpdatePostAsync(string id, PostInput postInput);
		Task<bool> DeletePostAsync(string id);
	}
}
