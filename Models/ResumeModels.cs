namespace ResumePDFAnalyzer.Models;

public class ResumeProfile
{
    public string ResumeText { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string RequiredSkills { get; set; } = string.Empty;
    public string Qualifications { get; set; } = string.Empty;
}

public class StartAnalysisRequest
{
    public ResumeProfile Profile { get; set; } = new();
}

public class ContinueChatRequest
{
    public string SessionId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class ChatResponse
{
    public string SessionId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsAnalysisComplete { get; set; }
}

public class ReportRequest
{
    public string SessionId { get; set; } = string.Empty;
}

public class ResumeReportResponse
{
    public string SessionId { get; set; } = string.Empty;
    public int Score { get; set; }
    public string Suitability { get; set; } = string.Empty;
    public List<string> Gaps { get; set; } = [];
    public string RecruiterAdvice { get; set; } = string.Empty;
    public List<string> SuggestedQuestions { get; set; } = [];
}

public class CandidateInfoRequest
{
    public string ResumeText { get; set; } = string.Empty;
}

public class CandidateInfoResponse
{
    public string Name { get; set; } = string.Empty;
    public string Age { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Tel { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
}
