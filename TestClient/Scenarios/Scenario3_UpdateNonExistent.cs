using System.Text;
using System.Text.Json;
using TestClient.Models;

namespace TestClient.Scenarios
{
    public static class Scenario3_UpdateNonExistent
    {
        public static async Task<List<TestResult>> Run()
        {
            var results = new List<TestResult>();
            var httpClient = new HttpClient();
            string baseUrl = Environment.GetEnvironmentVariable("API_BASE_URL") ?? "http://localhost:8080";
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };

            // Healthcheck
            results.Add(await Scenario1_FullCrud.HealthCheck(httpClient, baseUrl));

            // Update non-existent post
            var updateInput = new PostInput
            {
                Author = "Ghost",
                Date = DateTime.UtcNow,
                Content = "This post does not exist."
            };
            var content = new StringContent(JsonSerializer.Serialize(updateInput, jsonOptions), Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync($"{baseUrl}/posts/does-not-exist", content);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                results.Add(new TestResult("Update Non-existent Post", true, "Correctly returned 404"));
            }
            else
            {
                results.Add(new TestResult("Update Non-existent Post", false, $"Expected 404, got {response.StatusCode}"));
            }

            return results;
        }
    }
}