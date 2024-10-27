using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Parser.Models;

public class SiteParseRule
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string siteName { get; set; }
    public PageWithVacanciesParseRule PageWithVacanciesParseRule { get; set; }
    public VacancyParseRule VacancyParseRule { get; set; }
}