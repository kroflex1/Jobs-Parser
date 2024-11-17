using Parser.Models;

namespace Parser.Services;

public interface IParseRulesService
{
    Task<List<SiteParseRule>> GetAllParseRules();
    Task<SiteParseRule> GetParseRuleById(int id);
    Task UpdateParseRule(int id, SiteParseRule updatedSite);
}