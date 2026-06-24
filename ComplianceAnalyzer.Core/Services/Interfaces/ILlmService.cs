using ComplianceAnalyzer.Core.Models;

namespace ComplianceAnalyzer.Core.Interfaces;

public interface ILlmService
{
    Task<RuleVerdict> EvaluateRuleAsync(
        ComplianceRule rule,
        List<string> relevantPassages);
}
