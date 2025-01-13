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
        fillDefaultValuesForHeadHunter(modelBuilder);
        fillDefaultValueForSuperJob(modelBuilder);
    }

    private void fillDefaultValueForSuperJob(ModelBuilder modelBuilder)
    {
        Guid pageWithVacanciesParseRuleId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        Guid vacancyParseRuleId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        Guid pageWithResumesParseRuleId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        Guid resumeParseRuleId = Guid.Parse("00000000-0000-0000-0000-000000000002");

        modelBuilder.Entity<PageWithVacanciesParseRule>().HasData(
            new PageWithVacanciesParseRule
            {
                Id = pageWithVacanciesParseRuleId,
                Rules = JsonSerializer.Serialize(new
                {
                    UrlWithVacancies = "https://www.superjob.ru/vacancy/search",
                    ParamNameForVacancyTitle = "keywords",
                    ParamNameForVacanciesWithSalary = "payment_defined",
                    ParamNameForRegion = "geo",
                    VacancyUrlNode = "//div[@class='f-test-search-result-item']/div/div/div/div/div/div//a[contains(@class, 'vhN9a')]",
                    NextPageNode = "//a[contains(@class, 'f-test-button-dalshe')]"
                })
            }
        );

        modelBuilder.Entity<VacancyParseRule>().HasData(
            new VacancyParseRule
            {
                Id = vacancyParseRuleId,
                Rules = JsonSerializer.Serialize(new
                {
                    CompanyNameNode = "//div[@class='CgUNH _2ASwq _1wr1m _3ZU2u S5lKl']/div/div/div/div/a/span",
                    NameNode = "//h1[@class='_4zi0R _1wr1m _26u2P S5lKl _18w7M _1Ybm2 _1vuA_ gIJzo']",
                    CityNode = "//div[@class='CgUNH _4zi0R _1wr1m _26u2P S5lKl Oc7ey']//span[@class='_2AOqy _1Ybm2 _3Owqe _3GqZ-']",
                    DescriptionNode = "//div[@class='ymnAz _3ReRI ETQBj NMqQC']/div[2]",
                    SalaryNode = "//span[@class='kk-+S _18w7M _1Ybm2 _3GqZ-']",
                    CreationTimeNode = "//div[contains(@class, 'f-test-vacancy-base-info')][1]/div/div/div[@class='v3Bti'][2]/span"
                })
            }
        );

        modelBuilder.Entity<PageWithResumesParseRule>().HasData(
            new PageWithResumesParseRule
            {
                Id = pageWithResumesParseRuleId,
                Rules = JsonSerializer.Serialize(new
                {
                    UrlWithResumes = "https://www.superjob.ru/resume/search_resume.html", 
                    ParamNameForResumeTitle = "keywords",
                    ParamNameForRegion = "r",
                    ParamNameForTown = "t",
                    ResumeUrlNode = "//div[contains(@class, 'f-test-resume-snippet')]/div/div/div/div/div/div[2]/div[2]/a[1]",
                    NextPageNode = "//a[contains(@class, 'f-test-button-dalshe')]"
                })
            }
        );

        modelBuilder.Entity<ResumeParseRule>().HasData(
            new ResumeParseRule
            {
                Id = resumeParseRuleId,
                Rules = JsonSerializer.Serialize(new
                {
                    RoleNode = "//h1[@class='VB8-V _2J9pJ yl-Ea _1r5YN']",
                    SalaryNode = "//span[@class='_2J9pJ yl-Ea _2brpX']",
                    CityNode = "//div[@class='J+R2u']"
                })
            }
        );

        modelBuilder.Entity<SiteParseRule>().HasData(
            new SiteParseRule
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                SiteName = "superjob",
                PageWithVacanciesParseRuleId = pageWithVacanciesParseRuleId,
                VacancyParseRuleId = vacancyParseRuleId,
                PageWithResumesParseRuleId = pageWithResumesParseRuleId,
                ResumeParseRuleId = resumeParseRuleId
            }
        );
    }

    private void fillDefaultValuesForHeadHunter(ModelBuilder modelBuilder)
    {
        Guid pageWithVacanciesParseRuleId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        Guid vacancyParseRuleId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        Guid pageWithResumesParseRuleId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        Guid resumeParseRuleId = Guid.Parse("00000000-0000-0000-0000-000000000001");

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
                    UrlWithResumes = "https://hh.ru/search/resume",
                    ParamNameForResumeTitle = "text",
                    ParamNameForSearchStatus = "job_search_status",
                    ParamNameForRegion = "area",
                    ResumeUrlNode = "//div[contains(@class, 'resume-card-content')]//a[@data-qa='serp-item__title']",
                    NextPageNode = "//a[@data-qa='pager-next']",
                })
            }
        );


        modelBuilder.Entity<ResumeParseRule>().HasData(
            new ResumeParseRule
            {
                Id = resumeParseRuleId,
                Rules = JsonSerializer.Serialize(new
                {
                    RoleNode = "//span[@class='resume-block__title-text']",
                    PhoneContactNode = "//div[@data-qa='resume-contacts-phone']/a",
                    EmailContactNode = "//div[@data-qa='resume-contact-email']",
                    PersonalContactNode = "//div[@data-qa='resume-personalsite-personal']/a",
                    SalaryNode = "//span[@class='resume-block__salary']",
                    CityNode = "//span[@data-qa='resume-personal-address']"
                })
            }
        );

        modelBuilder.Entity<SiteParseRule>().HasData(
            new SiteParseRule
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                SiteName = "hh.ru",
                PageWithVacanciesParseRuleId = pageWithVacanciesParseRuleId,
                VacancyParseRuleId = vacancyParseRuleId,
                PageWithResumesParseRuleId = pageWithResumesParseRuleId,
                ResumeParseRuleId = resumeParseRuleId
            }
        );
    }
}