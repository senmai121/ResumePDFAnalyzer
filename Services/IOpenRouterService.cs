using ResumePDFAnalyzer.Models;

namespace ResumePDFAnalyzer.Services;

public interface IOpenRouterService
{
    Task<string> ChatAsync(List<OpenRouterMessage> messages, CancellationToken cancellationToken = default);
}
