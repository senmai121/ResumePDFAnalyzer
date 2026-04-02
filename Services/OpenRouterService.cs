using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ResumePDFAnalyzer.Models;

namespace ResumePDFAnalyzer.Services;

public class OpenRouterService : IOpenRouterService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public OpenRouterService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenRouter:ApiKey"]
            ?? throw new InvalidOperationException("OpenRouter:ApiKey is not configured.");
        _model = configuration["OpenRouter:Model"] ?? "google/gemini-2.0-flash-001";
    }

    public async Task<string> ChatAsync(List<OpenRouterMessage> messages, CancellationToken cancellationToken = default)
    {
        var request = new OpenRouterRequest
        {
            Model = _model,
            Messages = messages,
            MaxTokens = 2048,
            Temperature = 0.7
        };

        var json = JsonSerializer.Serialize(request, JsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://openrouter.ai/api/v1/chat/completions");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        requestMessage.Headers.Add("HTTP-Referer", "https://resume-analyzer.local");
        requestMessage.Headers.Add("X-Title", "Resume Analyzer Agent");
        requestMessage.Content = content;

        var response = await _httpClient.SendAsync(requestMessage, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        Console.WriteLine($"[OpenRouter] {(int)response.StatusCode} {response.StatusCode}");

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"OpenRouter error {(int)response.StatusCode}: {responseBody}");

        var result = JsonSerializer.Deserialize<OpenRouterResponse>(responseBody, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize OpenRouter response.");

        return result.Choices.FirstOrDefault()?.Message.Content
            ?? throw new InvalidOperationException("OpenRouter returned empty response.");
    }
}
