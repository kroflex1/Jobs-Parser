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
        base.OnModelCreating(modelBuilder);
        fillWithDefaultValues(modelBuilder);
    }
    
    private void fillWithDefaultValues(ModelBuilder modelBuilder)
    {
        Guid pageWithVacanciesParseRuleId = Guid.NewGuid();
        Guid vacancyParsuRuleId = Guid.NewGuid();
        
        modelBuilder.Entity<PageWithVacanciesParseRule>().HasData(
            new PageWithVacanciesParseRule
            {
                Id = pageWithVacanciesParseRuleId,
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
                Id = vacancyParsuRuleId,
                CompanyNameNode = "//a[@data-qa='vacancy-company-name']/span/span",
                NameNode = "//h1[@data-qa='vacancy-title']",
                CityNode = "//span[@data-qa='vacancy-view-raw-address'] | //p[@data-qa='vacancy-view-location']",
                DescriptionNode = "//div[@data-qa='vacancy-description'",
                SalaryNode = "//span[@data-qa='vacancy-salary-compensation-type-net']",
                CreationTimeNode = "//p[@class='vacancy-creation-time-redesigned']/span"
            }
        );
    
        modelBuilder.Entity<SiteParseRule>().HasData(
            new SiteParseRule
            {
                Id = Guid.NewGuid(),
                SiteName = "hh.ru",
                PageWithVacanciesParseRuleId = pageWithVacanciesParseRuleId,
                VacancyParseRuleId = vacancyParsuRuleId
            }
        );
    }
}