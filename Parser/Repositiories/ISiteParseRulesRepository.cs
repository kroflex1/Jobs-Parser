using Parser.DTO;
using Parser.Models;

namespace Parser.Services;

public interface ISiteParseRulesRepository
{
    Task<List<SiteParseRule>> GetAllSiteParseRules();
    Task<SiteParseRule> GetSiteParseRuleById(Guid id);
    Task UpdateSiteParseRule(SiteParseRuleDTO updatedSiteParseRules);
}