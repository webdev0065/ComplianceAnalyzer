//using ComplianceAnalyzer.Core.Interfaces;
//using ComplianceAnalyzer.Core.Services;
//using ComplianceAnalyzer.Infrastructure.Data;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.OpenApi.Models;
//using System;

//var builder = WebApplication.CreateBuilder(args);

//// Add services
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "Compliance Document Analyzer API",
//        Version = "v1",
//        Description = "AI-powered audit and compliance document analysis"
//    });
//});

//// Database
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(
//        builder.Configuration.GetConnectionString("DefaultConnection"),
//        sqlOptions => sqlOptions.EnableRetryOnFailure()));

//// HTTP Client for LLM/Embedding services
//builder.Services.AddHttpClient<IEmbeddingService, EmbeddingService>();
//builder.Services.AddHttpClient<ILlmService, LlmService>();

//// Core services
//builder.Services.AddScoped<IDocumentParser, DocumentParser>();
//builder.Services.AddScoped<IAuditService, AuditService>();

//// CORS for React frontend
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("ReactApp", policy =>
//    {
//        policy.WithOrigins("[localhost](http://localhost:3000)", "[localhost](http://localhost:5173)")
//              .AllowAnyHeader()
//              .AllowAnyMethod();
//    });
//});

//var app = builder.Build();

//// Ensure database is created
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    db.Database.EnsureCreated();
//}

//// Configure pipeline
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
//app.UseCors("ReactApp");
//app.UseAuthorization();
//app.MapControllers();

//app.Run();
