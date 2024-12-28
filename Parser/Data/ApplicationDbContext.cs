using System.Text.Json;
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
    public DbSet<PageWithResumesParseRule> PageWithResumesParseRules { get; set; }
    public DbSet<ResumeParseRule> ResumeParseRule { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<PageWithVacanciesParseRule>()
            .Property(e => e.Rules)
            .HasColumnType("jsonb");
        modelBuilder.Entity<VacancyParseRule>()
            .Property(e => e.Rules)
            .HasColumnType("jsonb");
        modelBuilder.Entity<PageWithResumesParseRule>()
            .Property(e => e.Rules)
            .HasColumnType("jsonb");
        modelBuilder.Entity<ResumeParseRule>()
            .Property(e => e.Rules)
            .HasColumnType("jsonb");
        fillWithDefaultValues(modelBuilder);
    }

    private void fillWithDefaultValues(ModelBuilder modelBuilder)
    {
        Guid pageWithVacanciesParseRuleId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        Guid vacancyParseRuleId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        Guid pageWithResumesParseRuleId = Guid.Parse("00000000-0000-0000-0000-000000000003");
        Guid resumeParseRuleId = Guid.Parse("00000000-0000-0000-0000-000000000004");

        modelBuilder.Entity<PageWithVacanciesParseRule>().HasData(
            new PageWithVacanciesParseRule
            {
                Id = pageWithVacanciesParseRuleId,
                Rules = JsonSerializer.Serialize(new
                {
                    UrlWithVacancies = "https://hh.ru/search/vacancy",
                    ParamNameForVacancyTitle = "text",
                    ParamNameForVacanciesWithSalary = "only_with_salary",
                    ParamNameForRegion = "area",
                    VacancyUrlNode = "//div[contains(@class, 'vacancy-info')]//a[@data-qa='serp-item__title']",
                    NextPageNode = "//a[@data-qa='pager-next']"
                })
            }
        );

        modelBuilder.Entity<VacancyParseRule>().HasData(
            new VacancyParseRule
            {
                Id = vacancyParseRuleId,
                Rules = JsonSerializer.Serialize(new
                {
                    CompanyNameNode = "//a[@data-qa='vacancy-company-name']/span",
                    NameNode = "//h1[@data-qa='vacancy-title']",
                    CityNode = "//span[@data-qa='vacancy-view-raw-address'] | //p[@data-qa='vacancy-view-location']",
                    DescriptionNode = "//div[@data-qa='vacancy-description']",
                    SalaryNode = "//span[@data-qa='vacancy-salary-compensation-type-net']",
                    CreationTimeNode = "//p[@class='vacancy-creation-time-redesigned']/span"
                })
            }
        );

        modelBuilder.Entity<PageWithResumesParseRule>().HasData(
            new PageWithResumesParseRule
            {
                Id = pageWithResumesParseRuleId,
                Rules = JsonSerializer.Serialize(new
                {
                    UrlWithResumes = "",
                    ParamNameForResumeTitle = "",
                    ResumeUrlNode = "",
                    NextPageNode = "",
                })
            }
        );


        modelBuilder.Entity<ResumeParseRule>().HasData(
            new ResumeParseRule
            {
                Id = resumeParseRuleId,
                Rules = JsonSerializer.Serialize(new
                {
                    FullNameNode = "",
                    RoleNode = "",
                    ContactsNode = "",
                    CityNode = "",
                })
            }
        );

        modelBuilder.Entity<SiteParseRule>().HasData(
            new SiteParseRule
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000005"),
                SiteName = "hh.ru",
                PageWithVacanciesParseRuleId = pageWithVacanciesParseRuleId,
                VacancyParseRuleId = vacancyParseRuleId,
                PageWithResumesParseRuleId = pageWithResumesParseRuleId,
                ResumeParseRuleId = resumeParseRuleId
            }
        );
    }
}