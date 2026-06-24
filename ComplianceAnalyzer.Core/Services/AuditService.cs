using ComplianceAnalyzer.Core.Interfaces;
using ComplianceAnalyzer.Core.Models;

namespace ComplianceAnalyzer.Core.Services;

public class AuditService : IAuditService
{
    private readonly IDocumentParser _documentParser;
    private readonly IEmbeddingService _embeddingService;
    private readonly ILlmService _llmService;

    public AuditService(
        IDocumentParser documentParser,
        IEmbeddingService embeddingService,
        ILlmService llmService)
    {
        _documentParser = documentParser;
        _embeddingService = embeddingService;
        _llmService = llmService;
    }

    public async Task<AuditReport> AnalyzeDocumentAsync(AuditRequest request)
    {
        // 1. Parse and chunk the document
        var chunks = await _documentParser.ParseAndChunkAsync(request.DocumentContent);

        if (chunks.Count == 0)
        {
            throw new ArgumentException("Document is empty or could not be parsed.");
        }

        // 2. Get the checklist (custom or default)
        var rules = request.CustomRules ?? GetDefaultChecklist();

        // 3. Evaluate each rule
        var verdicts = new List<RuleVerdict>();

        foreach (var rule in rules)
        {
            // Find relevant passages using embeddings/retrieval
            var relevantChunks = await _embeddingService.FindRelevantChunksAsync(
                rule.Description,
                chunks,
                topK: 3);

            var passages = relevantChunks
                .Where(c => c.Score > 0.1) // Minimum relevance threshold
                .Select(c => c.Chunk)
                .ToList();

            // Evaluate with LLM
            var verdict = await _llmService.EvaluateRuleAsync(rule, passages);
            verdicts.Add(verdict);
        }

        // 4. Build the report
        return new AuditReport
        {
            DocumentName = request.DocumentName ?? "Unnamed Document",
            Verdicts = verdicts
        };
    }

    public List<ComplianceRule> GetDefaultChecklist()
    {
        return new List<ComplianceRule>
        {
            new()
            {
                Id = 1,
                RuleId = "DATA-001",
                Description = "The document must specify data retention periods and policies",
                Category = "Data Management"
            },
            new()
            {
                Id = 2,
                RuleId = "SEC-001",
                Description = "The document must include provisions for data encryption at rest and in transit",
                Category = "Security"
            },
            new()
            {
                Id = 3,
                RuleId = "PRIV-001",
                Description = "The document must define procedures for handling personal data subject requests (access, deletion, correction)",
                Category = "Privacy"
            },
            new()
            {
                Id = 4,
                RuleId = "INC-001",
                Description = "The document must outline incident response and breach notification procedures",
                Category = "Incident Response"
            },
            new()
            {
                Id = 5,
                RuleId = "AUD-001",
                Description = "The document must require regular audits or compliance reviews",
                Category = "Audit"
            }
        };
    }
}
