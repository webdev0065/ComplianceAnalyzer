namespace ComplianceAnalyzer.Core.Interfaces;

public interface IDocumentParser
{
    Task<List<string>> ParseAndChunkAsync(string content);
    Task<string> ExtractTextFromPdfAsync(Stream pdfStream);
}
