namespace ComplianceAnalyzer.Core.Models;

public class AuditReport
{
    public Guid ReportId { get; set; } = Guid.NewGuid();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public string DocumentName { get; set; } = string.Empty;
    public List<RuleVerdict> Verdicts { get; set; } = new();

    public int TotalRules => Verdicts.Count;
    public int MetCount => Verdicts.Count(v => v.Status == VerdictStatus.Met);
    public int MissingCount => Verdicts.Count(v => v.Status == VerdictStatus.Missing);
    public int NeedsReviewCount => Verdicts.Count(v => v.Status == VerdictStatus.NeedsReview);

    public double CompliancePercentage => TotalRules > 0
        ? Math.Round((double)MetCount / TotalRules * 100, 1)
        : 0;
}
