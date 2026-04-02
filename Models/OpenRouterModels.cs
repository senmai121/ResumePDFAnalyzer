namespace ResumePDFAnalyzer.Models;

public class OpenRouterRequest
{
    public string Model { get; set; } = string.Empty;
    public List<OpenRouterMessage> Messages { get; set; } = [];
    public int? MaxTokens { get; set; }
    public double? Temperature { get; set; }
}

public class OpenRouterMessage
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class OpenRouterResponse
{
    public List<OpenRouterChoice> Choices { get; set; } = [];
}

public class OpenRouterChoice
{
    public OpenRouterMessage Message { get; set; } = new();
}
