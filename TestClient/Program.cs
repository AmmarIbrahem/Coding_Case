using System.Collections;
using System.Text;
using System.Text.Json;
using TestClient.Models;

namespace TestClient
{
	class Program
	{
		private static readonly HttpClient httpClient = new HttpClient();

		private static string baseUrl = Environment.GetEnvironmentVariable("API_BASE_URL") ?? "http://localhost:8080";
		private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			WriteIndented = true
		};

		static async Task Main(string[] args)
		{
			// Wait for API to be ready
			await WaitForApiAsync();

			var testResults = new List<TestResult>();

			try
			{
				// Test 1: Get all posts (should have seed data)
				testResults.Add(await TestGetAllPosts("Initial seed data check"));

				// Test 2: Create a new post
				var newPost = await TestCreatePost();
				testResults.Add(newPost.Result);

				if (newPost.CreatedPost != null)
				{
					string postId = newPost.CreatedPost.Id;

					// Test 3: Get the created post by ID
					testResults.Add(await TestGetPostById(postId));

					// Test 4: Update the post
					testResults.Add(await TestUpdatePost(postId));

					// Test 5: Get all posts again (should show updated post)
					testResults.Add(await TestGetAllPosts("After update check"));

					// Test 6: Delete the post
					testResults.Add(await TestDeletePost(postId));

					// Test 7: Try to get deleted post (should return 404)
					testResults.Add(await TestGetDeletedPost(postId));
				}

				// Test 8: Try to get non-existent post
				testResults.Add(await TestGetNonExistentPost());

				// Test 9: Try to update non-existent post
				testResults.Add(await TestUpdateNonExistentPost());

				// Test 10: Try to delete non-existent post
				testResults.Add(await TestDeleteNonExistentPost());

			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Unexpected error: {ex.Message}");
				testResults.Add(new TestResult("Unexpected Error", false, ex.Message));
			}

			// Print summary
			PrintTestSummary(testResults);
		}

		static async Task WaitForApiAsync()
		{
			Console.WriteLine("⏳ Waiting for API to be ready...");
			int attempts = 0;
			int maxAttempts = 30;

			while (attempts < maxAttempts)
			{
				try
				{
					var response = await httpClient.GetAsync($"{baseUrl}/health");
					if (response.IsSuccessStatusCode)
					{
						Console.WriteLine("✅ API is ready!");
						return;
					}
				}
				catch
				{
					// API not ready yet
				}

				attempts++;
				await Task.Delay(2000); // Wait 2 seconds
				Console.WriteLine($"⏳ Attempt {attempts}/{maxAttempts}...");
			}

			throw new Exception("API did not become ready within timeout period");
		}

		static async Task<TestResult> TestGetAllPosts(string testName)
		{
			try
			{
				Console.WriteLine($"\n📋 {testName}");
				var response = await httpClient.GetAsync($"{baseUrl}/posts");

				if (response.StatusCode == System.Net.HttpStatusCode.OK)
				{
					var content = await response.Content.ReadAsStringAsync();
					var posts = JsonSerializer.Deserialize<Post[]>(content, jsonOptions);
					Console.WriteLine($"✅ GET /posts returned {posts?.Length ?? 0} posts");
					return new TestResult(testName, true, $"Found {posts?.Length ?? 0} posts");
				}
				else
				{
					Console.WriteLine($"❌ GET /posts failed with status: {response.StatusCode}");
					return new TestResult(testName, false, $"Status: {response.StatusCode}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ GET /posts threw exception: {ex.Message}");
				return new TestResult(testName, false, ex.Message);
			}
		}

		static async Task<(TestResult Result, Post? CreatedPost)> TestCreatePost()
		{
			try
			{
				Console.WriteLine("\n➕ Testing POST /posts");
				var newPostInput = new PostInput
				{
					Author = "Alice Johnson",
					Date = DateTime.Parse("2025-05-16T12:00:00Z"),
					Content = "This is a test post created by the test client."
				};

				var json = JsonSerializer.Serialize(newPostInput, jsonOptions);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				var response = await httpClient.PostAsync($"{baseUrl}/posts", content);

				if (response.StatusCode == System.Net.HttpStatusCode.Created)
				{
					var responseContent = await response.Content.ReadAsStringAsync();
					var createdPost = JsonSerializer.Deserialize<Post>(responseContent, jsonOptions);
					Console.WriteLine($"✅ POST /posts created post with ID: {createdPost?.Id}");
					return (new TestResult("Create Post", true, $"Created post ID: {createdPost?.Id}"), createdPost);
				}
				else
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					Console.WriteLine($"❌ POST /posts failed with status: {response.StatusCode}");
					Console.WriteLine($"Error: {errorContent}");
					return (new TestResult("Create Post", false, $"Status: {response.StatusCode}"), null);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ POST /posts threw exception: {ex.Message}");
				return (new TestResult("Create Post", false, ex.Message), null);
			}
		}

		static async Task<TestResult> TestGetPostById(string postId)
		{
			try
			{
				Console.WriteLine($"\n🔍 Testing GET /posts/{postId}");
				var response = await httpClient.GetAsync($"{baseUrl}/posts/{postId}");

				if (response.StatusCode == System.Net.HttpStatusCode.OK)
				{
					var content = await response.Content.ReadAsStringAsync();
					var post = JsonSerializer.Deserialize<Post>(content, jsonOptions);
					Console.WriteLine($"✅ GET /posts/{postId} returned post by {post?.Author}");
					return new TestResult("Get Post by ID", true, $"Retrieved post: {post?.Author}");
				}
				else
				{
					Console.WriteLine($"❌ GET /posts/{postId} failed with status: {response.StatusCode}");
					return new TestResult("Get Post by ID", false, $"Status: {response.StatusCode}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ GET /posts/{postId} threw exception: {ex.Message}");
				return new TestResult("Get Post by ID", false, ex.Message);
			}
		}

		static async Task<TestResult> TestUpdatePost(string postId)
		{
			try
			{
				Console.WriteLine($"\n✏️ Testing PUT /posts/{postId}");
				var updateInput = new PostInput
				{
					Author = "Alice Johnson (Updated)",
					Date = DateTime.Parse("2025-05-16T14:00:00Z"),
					Content = "This post has been updated by the test client."
				};

				var json = JsonSerializer.Serialize(updateInput, jsonOptions);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				var response = await httpClient.PutAsync($"{baseUrl}/posts/{postId}", content);

				if (response.StatusCode == System.Net.HttpStatusCode.OK)
				{
					var responseContent = await response.Content.ReadAsStringAsync();
					var updatedPost = JsonSerializer.Deserialize<Post>(responseContent, jsonOptions);
					Console.WriteLine($"✅ PUT /posts/{postId} updated post");
					return new TestResult("Update Post", true, "Post updated successfully");
				}
				else
				{
					Console.WriteLine($"❌ PUT /posts/{postId} failed with status: {response.StatusCode}");
					return new TestResult("Update Post", false, $"Status: {response.StatusCode}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ PUT /posts/{postId} threw exception: {ex.Message}");
				return new TestResult("Update Post", false, ex.Message);
			}
		}

		static async Task<TestResult> TestDeletePost(string postId)
		{
			try
			{
				Console.WriteLine($"\n🗑️ Testing DELETE /posts/{postId}");
				var response = await httpClient.DeleteAsync($"{baseUrl}/posts/{postId}");

				if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
				{
					Console.WriteLine($"✅ DELETE /posts/{postId} succeeded");
					return new TestResult("Delete Post", true, "Post deleted successfully");
				}
				else
				{
					Console.WriteLine($"❌ DELETE /posts/{postId} failed with status: {response.StatusCode}");
					return new TestResult("Delete Post", false, $"Status: {response.StatusCode}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ DELETE /posts/{postId} threw exception: {ex.Message}");
				return new TestResult("Delete Post", false, ex.Message);
			}
		}

		static async Task<TestResult> TestGetDeletedPost(string postId)
		{
			try
			{
				Console.WriteLine($"\n🔍 Testing GET deleted post /posts/{postId}");
				var response = await httpClient.GetAsync($"{baseUrl}/posts/{postId}");

				if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
					Console.WriteLine($"✅ GET /posts/{postId} correctly returned 404");
					return new TestResult("Get Deleted Post (404)", true, "Correctly returned 404");
				}
				else
				{
					Console.WriteLine($"❌ GET /posts/{postId} should have returned 404, got: {response.StatusCode}");
					return new TestResult("Get Deleted Post (404)", false, $"Expected 404, got {response.StatusCode}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ GET /posts/{postId} threw exception: {ex.Message}");
				return new TestResult("Get Deleted Post (404)", false, ex.Message);
			}
		}

		static async Task<TestResult> TestGetNonExistentPost()
		{
			try
			{
				Console.WriteLine("\n🔍 Testing GET non-existent post");
				var response = await httpClient.GetAsync($"{baseUrl}/posts/non-existent-id");

				if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
					Console.WriteLine("✅ GET non-existent post correctly returned 404");
					return new TestResult("Get Non-existent Post", true, "Correctly returned 404");
				}
				else
				{
					Console.WriteLine($"❌ GET non-existent post should return 404, got: {response.StatusCode}");
					return new TestResult("Get Non-existent Post", false, $"Expected 404, got {response.StatusCode}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ GET non-existent post threw exception: {ex.Message}");
				return new TestResult("Get Non-existent Post", false, ex.Message);
			}
		}

		static async Task<TestResult> TestUpdateNonExistentPost()
		{
			try
			{
				Console.WriteLine("\n✏️ Testing PUT non-existent post");
				var updateInput = new PostInput
				{
					Author = "Test Author",
					Date = DateTime.UtcNow,
					Content = "Test content"
				};

				var json = JsonSerializer.Serialize(updateInput, jsonOptions);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				var response = await httpClient.PutAsync($"{baseUrl}/posts/non-existent-id", content);

				if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
					Console.WriteLine("✅ PUT non-existent post correctly returned 404");
					return new TestResult("Update Non-existent Post", true, "Correctly returned 404");
				}
				else
				{
					Console.WriteLine($"❌ PUT non-existent post should return 404, got: {response.StatusCode}");
					return new TestResult("Update Non-existent Post", false, $"Expected 404, got {response.StatusCode}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ PUT non-existent post threw exception: {ex.Message}");
				return new TestResult("Update Non-existent Post", false, ex.Message);
			}
		}

		static async Task<TestResult> TestDeleteNonExistentPost()
		{
			try
			{
				Console.WriteLine("\n🗑️ Testing DELETE non-existent post");
				var response = await httpClient.DeleteAsync($"{baseUrl}/posts/non-existent-id");

				if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
					Console.WriteLine("✅ DELETE non-existent post correctly returned 404");
					return new TestResult("Delete Non-existent Post", true, "Correctly returned 404");
				}
				else
				{
					Console.WriteLine($"❌ DELETE non-existent post should return 404, got: {response.StatusCode}");
					return new TestResult("Delete Non-existent Post", false, $"Expected 404, got {response.StatusCode}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ DELETE non-existent post threw exception: {ex.Message}");
				return new TestResult("Delete Non-existent Post", false, ex.Message);
			}
		}

		static void PrintTestSummary(List<TestResult> results)
		{
			Console.WriteLine("\n" + new string('=', 50));
			Console.WriteLine("📊 TEST SUMMARY");
			Console.WriteLine(new string('=', 50));

			int passed = results.Count(r => r.Success);
			int failed = results.Count - passed;

			foreach (var result in results)
			{
				string status = result.Success ? "✅ PASS" : "❌ FAIL";
				Console.WriteLine($"{status} - {result.TestName}: {result.Details}");
			}

			Console.WriteLine(new string('-', 50));
			Console.WriteLine($"✅ PASSED: {passed}");
			Console.WriteLine($"❌ FAILED: {failed}");
			Console.WriteLine($"📊 TOTAL:  {results.Count}");

			if (failed == 0)
			{
				Console.WriteLine("\n🎉 ALL TESTS PASSED! 🎉");
			}
			else
			{
				Console.WriteLine($"\n⚠️  {failed} test(s) failed. Please check the logs above.");
				Environment.Exit(1);
			}
		}
	}

	public class TestResult
	{
		public string TestName { get; set; }
		public bool Success { get; set; }
		public string Details { get; set; }

		public TestResult(string testName, bool success, string details)
		{
			TestName = testName;
			Success = success;
			Details = details;
		}
	}
}