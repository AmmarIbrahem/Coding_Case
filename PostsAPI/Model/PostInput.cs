using System.ComponentModel.DataAnnotations;

namespace PostsAPI.Models
{
	public class PostInput
	{
		[Required]
		public string Author { get; set; } = string.Empty;

		[Required]
		public DateTime Date { get; set; }

		[Required]
		public string Content { get; set; } = string.Empty;
	}
}