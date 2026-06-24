namespace ComplianceAnalyzer.Core.Models;

public class AuditRequest
{
    public string DocumentContent { get; set; } = string.Empty;
    public string? DocumentName { get; set; }
    public List<ComplianceRule>? CustomRules { get; set; }
}
