using ComplianceAnalyzer.Core.Interfaces;

namespace ComplianceAnalyzer.Core.Services;

public class DocumentParser : IDocumentParser
{
    private const int ChunkSize = 500;
    private const int ChunkOverlap = 50;

    public Task<List<string>> ParseAndChunkAsync(string content)
    {
        var chunks = new List<string>();

        if (string.IsNullOrWhiteSpace(content))
            return Task.FromResult(chunks);

        // Split into paragraphs first
        var paragraphs = content
            .Split(new[] { "\n\n", "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Trim())
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .ToList();

        foreach (var paragraph in paragraphs)
        {
            if (paragraph.Length <= ChunkSize)
            {
                chunks.Add(paragraph);
            }
            else
            {
                // Split large paragraphs with overlap
                for (int i = 0; i < paragraph.Length; i += ChunkSize - ChunkOverlap)
                {
                    var length = Math.Min(ChunkSize, paragraph.Length - i);
                    chunks.Add(paragraph.Substring(i, length));
                }
            }
        }

        return Task.FromResult(chunks);
    }

    public Task<string> ExtractTextFromPdfAsync(Stream pdfStream)
    {
        // For a real implementation, use a library like PdfPig or iTextSharp
        // Stubbed for the 2-hour challenge
        throw new NotImplementedException(
            "PDF parsing requires PdfPig or similar library. " +
            "For this demo, please submit plain text.");
    }
}
