using Microsoft.EntityFrameworkCore;
using Parser.Data;
using Parser.DTO;
using Parser.Models;

namespace Parser.Services;

public class SiteParseRulesService : ISiteParseRulesService
{
    private readonly ApplicationDbContext _dbContext;

    public SiteParseRulesService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<SiteParseRule>> GetAllSiteParseRules()
    {
        return await _dbContext.SiteParseRules
            .Include(s => s.PageWithVacanciesParseRule)
            .Include(s => s.VacancyParseRule)
            .Include(s => s.PageWithResumesParseRule)
            .Include(s => s.ResumeParseRule)
            .ToListAsync();
    }


    public async Task<SiteParseRule> GetSiteParseRuleById(Guid id)
    {
        return await _dbContext.SiteParseRules
            .Include(s => s.PageWithVacanciesParseRule)
            .Include(s => s.VacancyParseRule)
            .Include(s => s.PageWithResumesParseRule)
            .Include(s => s.ResumeParseRule)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task UpdateSiteParseRule(SiteParseRuleDTO updatedSiteParseRules)
    {
        var existingSite = await _dbContext.SiteParseRules
            .Include(s => s.PageWithVacanciesParseRule)
            .Include(s => s.VacancyParseRule)
            .Include(s => s.PageWithResumesParseRule)
            .Include(s => s.ResumeParseRule)
            .FirstOrDefaultAsync(s => s.Id == updatedSiteParseRules.Id);

        if (existingSite == null)
        {
            throw new KeyNotFoundException($"SiteParseRule with id {updatedSiteParseRules.Id} not found.");
        }

        existingSite.PageWithVacanciesParseRule.Rules = updatedSiteParseRules.PageWithVacanciesParseRule.GetRawText();
        existingSite.VacancyParseRule.Rules = updatedSiteParseRules.VacancyParseRule.GetRawText();
        existingSite.PageWithResumesParseRule.Rules = updatedSiteParseRules.PageWithResumesParseRule.GetRawText();
        existingSite.ResumeParseRule.Rules = updatedSiteParseRules.ResumeParseRule.GetRawText();

        _dbContext.SiteParseRules.Update(existingSite);
        await _dbContext.SaveChangesAsync();
    }
}