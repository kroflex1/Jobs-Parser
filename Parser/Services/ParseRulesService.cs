using Microsoft.EntityFrameworkCore;
using Parser.Data;
using Parser.Models;

namespace Parser.Services;

public class ParseRulesService: IParseRulesService
{
    private readonly ApplicationDbContext _dbContext;

    public ParseRulesService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<SiteParseRule>> GetAllParseRules()
    {
        return await _dbContext.SiteParseRules
            .Include(s => s.PageWithVacanciesParseRule)
            .Include(s => s.VacancyParseRule)
            .ToListAsync();
    }
    

    public async Task<SiteParseRule> GetParseRuleById(Guid id)
    {
        return await _dbContext.SiteParseRules
            .Include(s => s.PageWithVacanciesParseRule)
            .Include(s => s.VacancyParseRule)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task UpdateParseRule(Guid id, SiteParseRule updatedSite)
    {
        var existingSite = await _dbContext.SiteParseRules.FindAsync(id);
        if (existingSite == null)
        {
            throw new KeyNotFoundException($"SiteParseRule with id {id} not found.");
        }
        
        existingSite.SiteName = updatedSite.SiteName;
        existingSite.PageWithVacanciesParseRule = updatedSite.PageWithVacanciesParseRule;
        existingSite.VacancyParseRule = updatedSite.VacancyParseRule;

        _dbContext.SiteParseRules.Update(existingSite);
        await _dbContext.SaveChangesAsync();
    }
}