using MongoDB.Driver;
using Parser.Models;

namespace Parser.Services;

public class SiteParseRulesService
{
    private readonly IMongoCollection<SiteParseRule> _sitesCollection;

    public SiteParseRulesService(MongoDbService mongoDbService)
    {
        _sitesCollection = mongoDbService.GetCollection<SiteParseRule>("sites");
    }

    public List<SiteParseRule> GetParseRules()
    {
        return _sitesCollection.Find(_ => true).ToList();
    }

    public SiteParseRule GetParseRuleById(string id)
    {
        return _sitesCollection.Find(site => site.Id == id).FirstOrDefault();
    }

    public void UpdateParseRule(string id, SiteParseRule updatedSite)
    {
        _sitesCollection.ReplaceOne(site => site.Id == id, updatedSite);
    }
}