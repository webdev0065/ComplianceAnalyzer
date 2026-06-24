namespace ComplianceAnalyzer.Core.Interfaces;

public interface IEmbeddingService
{
    Task<float[]> GetEmbeddingAsync(string text);
    Task<List<(string Chunk, double Score)>> FindRelevantChunksAsync(
        string query,
        List<string> chunks,
        int topK = 3);
}
