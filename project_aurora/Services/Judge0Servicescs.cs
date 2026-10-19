using project_aurora.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace project_aurora.Services;

public class Judge0Service
{
    private readonly HttpClient _httpClient;

    public Judge0Service(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Judge0Result> SubmitAsync(
        string sourceCode,
        int languageId)
    {
        var submission = new
        {
            source_code = sourceCode,
            language_id = languageId
        };

        var url =
            "http://localhost:2358/submissions" +
            "?base64_encoded=false&wait=true";

        var response = await _httpClient.PostAsJsonAsync(
            url, submission);

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<Judge0Result>();

        return result ?? throw new InvalidOperationException(
            "Judge0 returned an empty response.");
    }
}
