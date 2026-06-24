using ComplianceAnalyzer.Core.Interfaces;
using ComplianceAnalyzer.Core.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

namespace ComplianceAnalyzer.Core.Services;

public class LlmService : ILlmService
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;
    private readonly bool _useStub;

    public LlmService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenAI:ApiKey"];
        _useStub = string.IsNullOrEmpty(_apiKey) || _apiKey == "your-api-key-here";
    }

    public async Task<RuleVerdict> EvaluateRuleAsync(
        ComplianceRule rule,
        List<string> relevantPassages)
    {
        if (_useStub)
        {
            return GetStubVerdict(rule, relevantPassages);
        }

        var prompt = BuildPrompt(rule, relevantPassages);

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

        var request = new
        {
            model = "gpt-4o",
            messages = new[]
            {
                new { role = "system", content = GetSystemPrompt() },
                new { role = "user", content = prompt }
            },
            temperature = 0.1,
            response_format = new { type = "json_object" }
        };

        var response = await _httpClient.PostAsJsonAsync(
            "[api.openai.com](https://api.openai.com/v1/chat/completions)",
            request);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        var content = result
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return ParseLlmResponse(rule, content!);
    }

    private static string GetSystemPrompt() => """
        You are a compliance auditor. Analyze document passages against compliance rules.
        
        Respond with JSON in this exact format:
        {
            "status": "Met" | "Missing" | "NeedsReview",
            "supporting_passage": "exact quote from the document or 'No evidence found'",
            "reasoning": "brief explanation of your verdict",
            "confidence": 0.0 to 1.0
        }
        
        Rules:
        - "Met": Clear evidence the requirement is satisfied
        - "Missing": No evidence found or clearly not addressed
        - "NeedsReview": Ambiguous, partial, or unclear evidence
        - Always cite the exact passage that supports your verdict
        - If no relevant passage exists, set supporting_passage to "No evidence found"
        """;

    private static string BuildPrompt(ComplianceRule rule, List<string> passages)
    {
        var passagesText = passages.Count > 0
            ? string.Join("\n\n---\n\n", passages)
            : "No relevant passages found in the document.";

        return $"""
            COMPLIANCE RULE TO CHECK:
            {rule.RuleId}: {rule.Description}
            
            RELEVANT DOCUMENT PASSAGES:
            {passagesText}
            
            Evaluate whether this compliance rule is Met, Missing, or Needs Review based on the passages above.
            """;
    }

    private static RuleVerdict ParseLlmResponse(ComplianceRule rule, string jsonResponse)
    {
        try
        {
            var json = JsonDocument.Parse(jsonResponse);
            var root = json.RootElement;

            var statusStr = root.GetProperty("status").GetString() ?? "NeedsReview";
            var status = statusStr switch
            {
                "Met" => VerdictStatus.Met,
                "Missing" => VerdictStatus.Missing,
                _ => VerdictStatus.NeedsReview
            };

            return new RuleVerdict
            {
                RuleId = rule.RuleId,
                RuleDescription = rule.Description,
                Status = status,
                SupportingPassage = root.GetProperty("supporting_passage").GetString()
                    ?? "No evidence found",
                Reasoning = root.GetProperty("reasoning").GetString() ?? "",
                ConfidenceScore = root.GetProperty("confidence").GetDouble()
            };
        }
        catch
        {
            return new RuleVerdict
            {
                RuleId = rule.RuleId,
                RuleDescription = rule.Description,
                Status = VerdictStatus.NeedsReview,
                SupportingPassage = "No evidence found",
                Reasoning = "Failed to parse LLM response",
                ConfidenceScore = 0
            };
        }
    }

    private static RuleVerdict GetStubVerdict(ComplianceRule rule, List<string> passages)
    {
        // Deterministic stub responses for demo/testing
        var hasEvidence = passages.Count > 0 &&
            passages.Any(p => p.Length > 50);

        var passage = passages.FirstOrDefault() ?? "No evidence found";

        // Simulate different verdicts based on rule ID hash
        var hash = rule.RuleId.GetHashCode();
        var status = (hash % 3) switch
        {
            0 when hasEvidence => VerdictStatus.Met,
            1 when hasEvidence => VerdictStatus.NeedsReview,
            _ => hasEvidence ? VerdictStatus.Met : VerdictStatus.Missing
        };

        return new RuleVerdict
        {
            RuleId = rule.RuleId,
            RuleDescription = rule.Description,
            Status = status,
            SupportingPassage = hasEvidence
                ? (passage.Length > 200 ? passage[..200] + "..." : passage)
                : "No evidence found",
            Reasoning = $"[STUB MODE] Simulated evaluation for rule {rule.RuleId}",
            ConfidenceScore = hasEvidence ? 0.85 : 0.3
        };
    }
}
