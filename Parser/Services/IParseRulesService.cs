using Parser.Models;

namespace Parser.Services;

public interface IParseRulesService
{
    Task<List<SiteParseRule>> GetAllParseRules();
    Task<SiteParseRule> GetParseRuleById(Guid id);
    Task UpdateParseRule(Guid id, SiteParseRule updatedSite);
}