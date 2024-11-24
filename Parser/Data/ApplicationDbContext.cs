using Microsoft.EntityFrameworkCore;
using Parser.Models;

namespace Parser.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

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
        
        fillWithDefaultValues(modelBuilder);
    }

    private void fillWithDefaultValues(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PageWithVacanciesParseRule>().HasData(
            new PageWithVacanciesParseRule
            {
                Id = 1,
                UrlWithVacancies = new Uri("https://example.com/jobs"),
                ParamNameForVacancyTitle = "title",
                ParamNameForVacanciesWithSalary = "only_with_salary",
                VacancyUrlNode = "//a[@class='vacancy']",
                PageNumberNode = "//div[@class='pagination']"
            }
        );

        modelBuilder.Entity<VacancyParseRule>().HasData(
            new VacancyParseRule
            {
                Id = 1,
                CompanyNameNode = "//a[@data-qa='vacancy-company-name']/span/span",
                NameNode = "//h1[@data-qa='vacancy-title']",
                CityNode = "//span[@data-qa='vacancy-view-raw-address'] | //p[@data-qa='vacancy-view-location']",
                DescriptionNode = "//div[@data-qa='vacancy-description'",
                GradeNode = "",
                SalaryNode = "//span[@data-qa='vacancy-salary-compensation-type-net']"
            }
        );

        modelBuilder.Entity<SiteParseRule>().HasData(
            new SiteParseRule
            {
                Id = 1,
                SiteName = "hh.ru",
                PageWithVacanciesParseRuleId = 1,
                VacancyParseRuleId = 1
            }
        );
    }
}