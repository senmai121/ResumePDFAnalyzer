using ResumePDFAnalyzer.Models;

namespace ResumePDFAnalyzer.Services;

public interface IPdfService
{
    PdfExtractResponse Extract(Stream pdfStream);
}
