using System.Collections;
using System.Text;
using System.Text.Json;
using TestClient.Models;
using TestClient.Scenarios;

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
			await WaitForApiAsync();

			var testResults = new List<TestResult>();

			// Run each scenario in order
			testResults.AddRange(await Scenario1_FullCrud.Run());
			testResults.AddRange(await Scenario2_DeleteNonExistent.Run());
			testResults.AddRange(await Scenario3_UpdateNonExistent.Run());

			PrintTestSummary(testResults);
		}

		static async Task WaitForApiAsync()
		{
			Console.WriteLine("Waiting for API to be ready...");
			int attempts = 0;
			int maxAttempts = 30;

			while (attempts < maxAttempts)
			{
				try
				{
					var response = await httpClient.GetAsync($"{baseUrl}/health");
					if (response.IsSuccessStatusCode)
					{
						Console.WriteLine("API is ready!");
						return;
					}
				}
				catch
				{
					// API not ready yet
				}

				attempts++;
				await Task.Delay(2000); // Wait 2 seconds
				Console.WriteLine($"Attempt {attempts}/{maxAttempts}...");
			}

			throw new Exception("API did not become ready within timeout period");
		}

		static void PrintTestSummary(List<TestResult> results)
		{
			Console.WriteLine("\n" + new string('=', 50));
			Console.WriteLine("TEST SUMMARY");
			Console.WriteLine(new string('=', 50));

			int passed = results.Count(r => r.Success);
			int failed = results.Count - passed;

			foreach (var result in results)
			{
				string status = result.Success ? "PASS" : " FAIL";
				Console.WriteLine($"{status} - {result.TestName}: {result.Details}");
			}

			Console.WriteLine(new string('-', 50));
			Console.WriteLine($"PASSED: {passed}");
			Console.WriteLine($"FAILED: {failed}");
			Console.WriteLine($"TOTAL:  {results.Count}");

			if (failed == 0)
			{
				Console.WriteLine("\nALL TESTS PASSED!");
			}
			else
			{
				Console.WriteLine($"\n{failed} test(s) failed. Please check the logs above.");
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