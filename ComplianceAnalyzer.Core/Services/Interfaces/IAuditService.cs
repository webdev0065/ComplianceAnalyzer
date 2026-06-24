using ComplianceAnalyzer.Core.Models;

namespace ComplianceAnalyzer.Core.Interfaces;

public interface IAuditService
{
    Task<AuditReport> AnalyzeDocumentAsync(AuditRequest request);
    List<ComplianceRule> GetDefaultChecklist();
}
