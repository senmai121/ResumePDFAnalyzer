using ResumePDFAnalyzer.Models;

namespace ResumePDFAnalyzer.Services;

public interface IResumeAgentService
{
    Task<ChatResponse> StartSessionAsync(ResumeProfile profile, CancellationToken cancellationToken = default);
    Task<ChatResponse> ContinueSessionAsync(string sessionId, string userMessage, CancellationToken cancellationToken = default);
    Task<ResumeReportResponse> GetReportAsync(string sessionId, CancellationToken cancellationToken = default);
    Task<CandidateInfoResponse> ExtractCandidateInfoAsync(string resumeText, CancellationToken cancellationToken = default);
}
