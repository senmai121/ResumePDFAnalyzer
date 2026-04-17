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
            var words = page.GetWords().ToList();
            string text;

            // Detect two-column layout: check if words cluster into two distinct X regions
            var midX = page.Width / 2.0;
            var leftWords = words.Where(w => w.BoundingBox.Centroid.X < midX).ToList();
            var rightWords = words.Where(w => w.BoundingBox.Centroid.X >= midX).ToList();
            var hasRightContent = rightWords.Count > words.Count * 0.2; // right column has >20% of words

            if (hasRightContent)
            {
                // Two-column: extract each column top-to-bottom, left column first
                var leftText = string.Join(" ", leftWords
                    .OrderByDescending(w => w.BoundingBox.Bottom)
                    .ThenBy(w => w.BoundingBox.Left)
                    .Select(w => w.Text));
                var rightText = string.Join(" ", rightWords
                    .OrderByDescending(w => w.BoundingBox.Bottom)
                    .ThenBy(w => w.BoundingBox.Left)
                    .Select(w => w.Text));
                text = leftText + "\n" + rightText;
            }
            else
            {
                // Single-column: preserve natural reading order
                text = string.Join(" ", words
                    .OrderByDescending(w => w.BoundingBox.Bottom)
                    .ThenBy(w => w.BoundingBox.Left)
                    .Select(w => w.Text));
            }

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
