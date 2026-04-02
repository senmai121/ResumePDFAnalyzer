using System.Text;
using ResumePDFAnalyzer.Models;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace ResumePDFAnalyzer.Services;

public class PdfService : IPdfService
{
    public PdfExtractResponse Extract(Stream pdfStream)
    {
        using var document = PdfDocument.Open(pdfStream);

        var pages = new List<PageText>();

        foreach (var page in document.GetPages())
        {
            var words = page.GetWords();
            var text = string.Join(" ", words.Select(w => w.Text));

            pages.Add(new PageText
            {
                PageNumber = page.Number,
                Text = text
            });
        }

        var fullText = new StringBuilder();
        foreach (var page in pages)
        {
            fullText.AppendLine(page.Text);
        }

        return new PdfExtractResponse
        {
            PageCount = pages.Count,
            FullText = fullText.ToString().Trim(),
            Pages = pages
        };
    }
}
