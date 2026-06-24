//using ComplianceAnalyzer.Core.Models;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Reflection.Emit;

//namespace ComplianceAnalyzer.Infrastructure.Data;

//public class AppDbContext : DbContext
//{
//    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
//    {
//    }

//    public DbSet<ComplianceRule> ComplianceRules => Set<ComplianceRule>();

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        modelBuilder.Entity<ComplianceRule>(entity =>
//        {
//            entity.HasKey(e => e.Id);
//            entity.Property(e => e.RuleId).HasMaxLength(50).IsRequired();
//            entity.Property(e => e.Description).HasMaxLength(500).IsRequired();
//            entity.Property(e => e.Category).HasMaxLength(100);
//            entity.HasIndex(e => e.RuleId).IsUnique();
//        });

//        // Seed default rules
//        modelBuilder.Entity<ComplianceRule>().HasData(
//            new ComplianceRule { Id = 1, RuleId = "DATA-001", Description = "The document must specify data retention periods and policies", Category = "Data Management" },
//            new ComplianceRule { Id = 2, RuleId = "SEC-001", Description = "The document must include provisions for data encryption at rest and in transit", Category = "Security" },
//            new ComplianceRule { Id = 3, RuleId = "PRIV-001", Description = "The document must define procedures for handling personal data subject requests", Category = "Privacy" },
//            new ComplianceRule { Id = 4, RuleId = "INC-001", Description = "The document must outline incident response and breach notification procedures", Category = "Incident Response" },
//            new ComplianceRule { Id = 5, RuleId = "AUD-001", Description = "The document must require regular audits or compliance reviews", Category = "Audit" }
//        );
//    }
//}
