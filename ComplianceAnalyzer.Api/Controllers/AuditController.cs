//using ComplianceAnalyzer.Core.Interfaces;
//using ComplianceAnalyzer.Core.Models;
//using Microsoft.AspNetCore.Mvc;

//namespace ComplianceAnalyzer.Api.Controllers;

//[ApiController]
//[Route("api/[controller]")]
//public class AuditController : ControllerBase
//{
//    private readonly IAuditService _auditService;
//    private readonly ILogger<AuditController> _logger;

//    public AuditController(IAuditService auditService, ILogger<AuditController> logger)
//    {
//        _auditService = auditService;
//        _logger = logger;
//    }

//    /// <summary>
//    /// Analyze a document against compliance rules
//    /// </summary>
//    [HttpPost("analyze")]
//    [ProducesResponseType(typeof(AuditReport), StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status400BadRequest)]
//    public async Task<ActionResult<AuditReport>> AnalyzeDocument([FromBody] AuditRequest request)
//    {
//        if (string.IsNullOrWhiteSpace(request.DocumentContent))
//        {
//            return BadRequest("Document content is required.");
//        }

//        try
//        {
//            _logger.LogInformation("Starting document analysis for: {DocumentName}",
//                request.DocumentName ?? "Unnamed");

//            var report = await _auditService.AnalyzeDocumentAsync(request);

//            _logger.LogInformation(
//                "Analysis complete. Met: {Met}, Missing: {Missing}, Review: {Review}",
//                report.MetCount, report.MissingCount, report.NeedsReviewCount);

//            return Ok(report);
//        }
//        catch (ArgumentException ex)
//        {
//            return BadRequest(ex.Message);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error analyzing document");
//            return StatusCode(500, "An error occurred while analyzing the document.");
//        }
//    }

//    /// <summary>
//    /// Get the default compliance checklist
//    /// </summary>
//    [HttpGet("checklist")]
//    [ProducesResponseType(typeof(List<ComplianceRule>), StatusCodes.Status200OK)]
//    public ActionResult<List<ComplianceRule>> GetChecklist()
//    {
//        return Ok(_auditService.GetDefaultChecklist());
//    }

//    /// <summary>
//    /// Health check endpoint
//    /// </summary>
//    [HttpGet("health")]
//    public ActionResult Health()
//    {
//        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
//    }
//}
