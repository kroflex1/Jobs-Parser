using Microsoft.EntityFrameworkCore;
using Parser.Data;
using Parser.Models;

namespace Parser.Services;

public class SiteParseRuleService
{
    private readonly ApplicationDbContext _context;

    public SiteParseRuleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<SiteParseRule> GetSiteParseRules()
    {
        return _context.SiteParseRules
            .Include(s => s.PageWithVacanciesParseRule)
            .Include(s => s.VacancyParseRule)
            .Include(s => s.PageWithResumesParseRule)
            .Include(s => s.ResumeParseRule)
            .ToList();
    }
}