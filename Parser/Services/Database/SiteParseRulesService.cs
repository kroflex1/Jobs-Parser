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

    public List<SiteParseRule> GetParseRules() => _sitesCollection.Find(_ => true).ToList();
}