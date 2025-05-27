using System.Text;
using System.Text.Json;
using TestClient.Models;

namespace TestClient.Scenarios
{
	public static class Scenario1_FullCrud
	{
		public static async Task<List<TestResult>> Run()
		{
			var results = new List<TestResult>();
			var httpClient = new HttpClient();
			string baseUrl = Environment.GetEnvironmentVariable("API_BASE_URL") ?? "http://localhost:8080";
			var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };

			// Healthcheck
			results.Add(await HealthCheck(httpClient, baseUrl));

			// Create post
			var newPost = new PostInput
			{
				Author = "Jane Doe",
				Date = DateTime.UtcNow,
				Content = "Hello, this is Jane's first post!"
			};
			var createContent = new StringContent(JsonSerializer.Serialize(newPost, jsonOptions), Encoding.UTF8, "application/json");
			var createResp = await httpClient.PostAsync($"{baseUrl}/posts", createContent);
			Post? createdPost = null;
			if (createResp.StatusCode == System.Net.HttpStatusCode.Created)
			{
				var respContent = await createResp.Content.ReadAsStringAsync();
				createdPost = JsonSerializer.Deserialize<Post>(respContent, jsonOptions);
				results.Add(new TestResult("Create Post", true, $"Created post ID: {createdPost?.Id}"));
			}
			else
			{
				results.Add(new TestResult("Create Post", false, $"Status: {createResp.StatusCode}"));
				return results;
			}

			// Get all posts and verify
			var getAllResp = await httpClient.GetAsync($"{baseUrl}/posts");
			if (getAllResp.StatusCode == System.Net.HttpStatusCode.OK)
			{
				var content = await getAllResp.Content.ReadAsStringAsync();
				var posts = JsonSerializer.Deserialize<Post[]>(content, jsonOptions);
				bool found = posts?.Any(p => p.Id == createdPost!.Id) ?? false;
				results.Add(new TestResult("Get All Posts", found, found ? "Created post is present" : "Created post not found"));
			}
			else
			{
				results.Add(new TestResult("Get All Posts", false, $"Status: {getAllResp.StatusCode}"));
			}

			// Update post and verify
			var updateInput = new PostInput
			{
				Author = "Jane Doe (Edited)",
				Date = DateTime.UtcNow,
				Content = "Jane updated her post!"
			};
			var updateContent = new StringContent(JsonSerializer.Serialize(updateInput, jsonOptions), Encoding.UTF8, "application/json");
			var updateResp = await httpClient.PutAsync($"{baseUrl}/posts/{createdPost!.Id}", updateContent);
			if (updateResp.StatusCode == System.Net.HttpStatusCode.OK)
			{
				var respContent = await updateResp.Content.ReadAsStringAsync();
				var updatedPost = JsonSerializer.Deserialize<Post>(respContent, jsonOptions);
				bool valid = updatedPost != null && updatedPost.Author == updateInput.Author && updatedPost.Content == updateInput.Content;
				results.Add(new TestResult("Update Post", valid, valid ? "Post updated and verified" : "Update response did not match input"));
			}
			else
			{
				results.Add(new TestResult("Update Post", false, $"Status: {updateResp.StatusCode}"));
			}

			// Delete post and verify
			var deleteResp = await httpClient.DeleteAsync($"{baseUrl}/posts/{createdPost.Id}");
			if (deleteResp.StatusCode == System.Net.HttpStatusCode.NoContent)
			{
				results.Add(new TestResult("Delete Post", true, "Post deleted successfully"));
			}
			else
			{
				results.Add(new TestResult("Delete Post", false, $"Status: {deleteResp.StatusCode}"));
			}

			// Verify deletion
			var verifyResp = await httpClient.GetAsync($"{baseUrl}/posts/{createdPost.Id}");
			if (verifyResp.StatusCode == System.Net.HttpStatusCode.NotFound)
			{
				results.Add(new TestResult("Verify Deletion", true, "Post not found after deletion"));
			}
			else
			{
				results.Add(new TestResult("Verify Deletion", false, $"Expected 404, got {verifyResp.StatusCode}"));
			}

			return results;
		}

		public static async Task<TestResult> HealthCheck(HttpClient httpClient, string baseUrl)
		{
			try
			{
				var response = await httpClient.GetAsync($"{baseUrl}/health");
				if (response.IsSuccessStatusCode)
					return new TestResult("Healthcheck", true, "API is healthy");
				return new TestResult("Healthcheck", false, $"Status: {response.StatusCode}");
			}
			catch (Exception ex)
			{
				return new TestResult("Healthcheck", false, ex.Message);
			}
		}
	}
}