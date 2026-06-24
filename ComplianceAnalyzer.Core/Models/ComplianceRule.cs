namespace ComplianceAnalyzer.Core.Models;

public class ComplianceRule
{
    public int Id { get; set; }
    public string RuleId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}
