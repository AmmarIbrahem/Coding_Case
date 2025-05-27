namespace TestClient.Scenarios
{
    public static class Scenario2_DeleteNonExistent
    {
        public static async Task<List<TestResult>> Run()
        {
            var results = new List<TestResult>();
            var httpClient = new HttpClient();
            string baseUrl = Environment.GetEnvironmentVariable("API_BASE_URL") ?? "http://localhost:8080";

            // Healthcheck
            results.Add(await Scenario1_FullCrud.HealthCheck(httpClient, baseUrl));

            // Delete non-existent post
            var response = await httpClient.DeleteAsync($"{baseUrl}/posts/does-not-exist");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                results.Add(new TestResult("Delete Non-existent Post", true, "Correctly returned 404"));
            }
            else
            {
                results.Add(new TestResult("Delete Non-existent Post", false, $"Expected 404, got {response.StatusCode}"));
            }

            return results;
        }
    }
}