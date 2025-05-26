using PostsAPI.Models;

namespace PostsAPI.Data
{
	public interface IPostRepository
	{
		Task<IEnumerable<Post>> GetAllAsync();
		Task<Post?> GetByIdAsync(string id);
		Task<Post> CreateAsync(Post post);
		Task<Post> UpdateAsync(Post post);
		Task<bool> DeleteAsync(string id);
	}
}