# AI-Powered Compliance Document Analyzer
An ASP.NET Core Web API that analyzes policy documents against compliance checklists using AI (RAG + LLM).
## Quick Start
### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB works fine)
- (Optional) OpenAI API key
### Run the API
```bash
cd ComplianceAnalyzer.Api
dotnet run
The API will start at [localhost](https://localhost:7001) (or [localhost](http://localhost:5000)).

Test with Swagger
Open [localhost](https://localhost:7001/swagger) to explore and test endpoints.

Test with cURL
bash


# Get the default checklist
curl [localhost](https://localhost:7001/api/audit/checklist)
# Analyze a document
curl -X POST [localhost](https://localhost:7001/api/audit/analyze) \
  -H "Content-Type: application/json" \
  -d '{
    "documentName": "ACME Data Policy",
    "documentContent": "Your policy text here..."
  }'
API Endpoints
Endpoint	Method	Description
/api/audit/analyze	POST	Analyze document against checklist
/api/audit/checklist	GET	Get default compliance rules
/api/audit/health	GET	Health check
Configuration
Set your OpenAI API key in appsettings.json:

json


{
  "OpenAI": {
    "ApiKey": "sk-..."
  }
}
No API key? The system runs in stub mode with simulated responses—architecture and wiring still work.

Architecture Decisions
Clean separation: Core business logic has no infrastructure dependencies
Interface-based DI: All services are injected via interfaces for testability
Stub mode: Works without API keys for demo/evaluation
Chunking with overlap: Ensures context isn't lost at boundaries
Retrieval-first: Relevant passages found before LLM evaluation (RAG pattern)
What I'd Add with More Time
PDF parsing with PdfPig
Vector database (Qdrant/Pinecone) for large documents
Caching for embeddings
Background job queue for large batches
More sophisticated chunking (semantic boundaries)
Unit and integration tests
AI Tooling Used
Claude (architecture design and boilerplate)
Verified: all service interfaces, DI wiring, model relationships, prompt engineering
