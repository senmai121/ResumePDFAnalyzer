using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumePDFAnalyzer.Models;
using ResumePDFAnalyzer.Services;

namespace ResumePDFAnalyzer.Controllers;

[ApiController]
[Route("api/resume")]
[Authorize]
public class ResumeAgentController(IResumeAgentService resumeAgentService) : ControllerBase
{
    /// <summary>
    /// Start a Resume analysis session
    /// Submit resumeText along with position, requiredSkills, and qualifications
    /// The agent will analyze or ask for additional information
    /// </summary>
    [HttpPost("analyze")]
    public async Task<ActionResult<ChatResponse>> Analyze(
        [FromBody] StartAnalysisRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Profile.ResumeText))
            return BadRequest(new { error = "Resume content is required" });

        if (string.IsNullOrWhiteSpace(request.Profile.Position))
            return BadRequest(new { error = "Job position is required" });

        var response = await resumeAgentService.StartSessionAsync(request.Profile, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Continue messaging in an existing session
    /// Used to answer the agent's questions or provide additional information
    /// </summary>
    [HttpPost("chat")]
    public async Task<ActionResult<ChatResponse>> Chat(
        [FromBody] ContinueChatRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await resumeAgentService.ContinueSessionAsync(
                request.SessionId,
                request.Message,
                cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieve the Resume analysis summary report as structured JSON
    /// Contains score, suitability, gaps, recruiterAdvice, and suggestedQuestions
    /// </summary>
    [HttpPost("report")]
    public async Task<ActionResult<ResumeReportResponse>> GetReport(
        [FromBody] ReportRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await resumeAgentService.GetReportAsync(
                request.SessionId,
                cancellationToken);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Extract general candidate information from Resume content using AI
    /// Returns name, age, title, email, and tel
    /// </summary>
    [HttpPost("candidate-info")]
    public async Task<ActionResult<CandidateInfoResponse>> GetCandidateInfo(
        [FromBody] CandidateInfoRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ResumeText))
            return BadRequest(new { error = "Resume content is required" });

        var response = await resumeAgentService.ExtractCandidateInfoAsync(request.ResumeText, cancellationToken);
        return Ok(response);
    }
}
