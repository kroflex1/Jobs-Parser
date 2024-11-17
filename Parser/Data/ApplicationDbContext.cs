using Microsoft.EntityFrameworkCore;
using Parser.Models;

namespace Parser.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<SiteParseRule> SiteParseRules { get; set; }
    public DbSet<PageWithVacanciesParseRule> PageWithVacanciesParseRules { get; set; }
    public DbSet<VacancyParseRule> VacancyParseRules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SiteParseRule>()
            .HasOne(s => s.PageWithVacanciesParseRule)
            .WithMany()
            .HasForeignKey(s => s.PageWithVacanciesParseRuleId);

        modelBuilder.Entity<SiteParseRule>()
            .HasOne(s => s.VacancyParseRule)
            .WithMany()
            .HasForeignKey(s => s.VacancyParseRuleId);
    }
}