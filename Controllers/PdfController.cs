using Microsoft.AspNetCore.Mvc;
using ResumePDFAnalyzer.Services;

namespace ResumePDFAnalyzer.Controllers;

[ApiController]
[Route("api/pdf")]
public class PdfController(IPdfService pdfService) : ControllerBase
{
    [HttpPost("extract")]
    [RequestSizeLimit(20 * 1024 * 1024)] // 20 MB
    public IActionResult Extract(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "Please attach a PDF file" });

        if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) &&
            file.ContentType != "application/pdf")
            return BadRequest(new { error = "Only PDF files are supported" });

        try
        {
            using var stream = file.OpenReadStream();
            var result = pdfService.Extract(stream);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PdfExtract] Error: {ex.Message}");
            return BadRequest(new { error = "Unable to read the PDF file. Please ensure the file is not encrypted or corrupted." });
        }
    }
}
