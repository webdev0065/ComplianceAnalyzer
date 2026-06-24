using ComplianceAnalyzer.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

namespace ComplianceAnalyzer.Core.Services;

public class EmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;
    private readonly bool _useStub;

    public EmbeddingService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenAI:ApiKey"];
        _useStub = string.IsNullOrEmpty(_apiKey) || _apiKey == "your-api-key-here";
    }

    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        if (_useStub)
        {
            return GetStubEmbedding(text);
        }

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

        var request = new
        {
            model = "text-embedding-3-small",
            input = text
        };

        var response = await _httpClient.PostAsJsonAsync(
            "[api.openai.com](https://api.openai.com/v1/embeddings)",
            request);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        var embedding = result
            .GetProperty("data")[0]
            .GetProperty("embedding")
            .EnumerateArray()
            .Select(e => e.GetSingle())
            .ToArray();

        return embedding;
    }

    public async Task<List<(string Chunk, double Score)>> FindRelevantChunksAsync(
        string query,
        List<string> chunks,
        int topK = 3)
    {
        if (_useStub)
        {
            // Return chunks based on simple keyword matching for stub
            return GetStubRelevantChunks(query, chunks, topK);
        }

        var queryEmbedding = await GetEmbeddingAsync(query);
        var chunkScores = new List<(string Chunk, double Score)>();

        foreach (var chunk in chunks)
        {
            var chunkEmbedding = await GetEmbeddingAsync(chunk);
            var similarity = CosineSimilarity(queryEmbedding, chunkEmbedding);
            chunkScores.Add((chunk, similarity));
        }

        return chunkScores
            .OrderByDescending(c => c.Score)
            .Take(topK)
            .ToList();
    }

    private static double CosineSimilarity(float[] a, float[] b)
    {
        double dotProduct = 0;
        double magnitudeA = 0;
        double magnitudeB = 0;

        for (int i = 0; i < a.Length; i++)
        {
            dotProduct += a[i] * b[i];
            magnitudeA += a[i] * a[i];
            magnitudeB += b[i] * b[i];
        }

        return dotProduct / (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
    }

    private static float[] GetStubEmbedding(string text)
    {
        // Generate deterministic pseudo-embedding based on text hash
        var hash = text.GetHashCode();
        var random = new Random(hash);
        return Enumerable.Range(0, 256)
            .Select(_ => (float)(random.NextDouble() * 2 - 1))
            .ToArray();
    }

    private static List<(string Chunk, double Score)> GetStubRelevantChunks(
        string query,
        List<string> chunks,
        int topK)
    {
        // Simple keyword-based relevance for stub mode
        var queryWords = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return chunks
            .Select(chunk =>
            {
                var chunkLower = chunk.ToLower();
                var matchCount = queryWords.Count(w => chunkLower.Contains(w));
                var score = (double)matchCount / queryWords.Length;
                return (chunk, score);
            })
            .OrderByDescending(c => c.score)
            .Take(topK)
            .ToList();
    }
}
