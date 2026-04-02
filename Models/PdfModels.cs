namespace ResumePDFAnalyzer.Models;

public class PageText
{
    public int PageNumber { get; set; }
    public string Text { get; set; } = "";
}

public class PdfExtractResponse
{
    public int PageCount { get; set; }
    public string FullText { get; set; } = "";
    public List<PageText> Pages { get; set; } = [];
}
