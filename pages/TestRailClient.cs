using System.Net;
using RestSharp;
using RestSharp.Authenticators;
using AltTester;

public class TestRailClient
{
    private readonly string _baseUrl;
    private readonly string _username;
    private readonly string _password;
    private readonly RestClient client;
    private readonly List<Dictionary<string, object>> testResults = new();

    public TestRailClient(string baseUrl, string username, string password)
    {
        _baseUrl = baseUrl;
        _username = username;
        _password = password;
        
        var options = new RestClientOptions(baseUrl)
        {
            Authenticator = new HttpBasicAuthenticator(username, password)
        };
        client = new RestClient(options);
    }

    public void QueueTestResult(int caseId, int statusId, string comment)
    {
        testResults.Add(new Dictionary<string, object>
        {
            { "case_id", caseId },
            { "status_id", statusId },
            { "comment", comment }
        });
    }

    public void SubmitResults(int runId)
    {
        if (testResults.Count == 0)
        {
            Console.WriteLine("⚠️ No test results to submit.");
            return;
        }

        var request = new RestRequest($"/index.php?/api/v2/add_results_for_cases/{runId}", Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddJsonBody(new { results = testResults });

        var response = client.Execute(request);
        if (response.StatusCode == HttpStatusCode.OK)
            Console.WriteLine("✅ Test results added successfully at the end of test run.");
        else
            Console.WriteLine($"❌ Failed to add test results: {response.Content}");
    }

    public void AddAttachmentToRun(int runId, string filePath)
    {
        var request = new RestRequest($"/index.php?/api/v2/add_attachment_to_run/{runId}", Method.Post);
        request.AddFile("attachment", filePath);

        var response = client.Execute(request);
        if (response.StatusCode == HttpStatusCode.OK)
            Console.WriteLine("✅ Attachment added to test run successfully.");
        else
            Console.WriteLine($"❌ Failed to add attachment to test run: {response.Content}");
    }
}
