namespace TestClient.Models
{
	public class Post
	{
		public string Id { get; set; } = string.Empty;
		public string Author { get; set; } = string.Empty;
		public DateTime Date { get; set; }
		public string Content { get; set; } = string.Empty;
	}

	public class PostInput
	{
		public string Author { get; set; } = string.Empty;
		public DateTime Date { get; set; }
		public string Content { get; set; } = string.Empty;
	}
}