namespace ComplianceAnalyzer.Core.Models;

public enum VerdictStatus
{
    Met,
    Missing,
    NeedsReview
}

public class RuleVerdict
{
    public string RuleId { get; set; } = string.Empty;
    public string RuleDescription { get; set; } = string.Empty;
    public VerdictStatus Status { get; set; }
    public string SupportingPassage { get; set; } = string.Empty;
    public string Reasoning { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
}
