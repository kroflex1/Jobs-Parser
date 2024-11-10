using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Parser.Models;

/// <summary>
/// Представляет правила парсинга сайта.
/// </summary>
public class SiteParseRule
{
    /// <summary>
    /// Уникальный идентификатор правила парсинга для определенного сайта
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    /// <summary>
    /// Название сайта, для которого установлены правила парсинга
    /// </summary>
    public string SiteName { get; set; }
    
    /// <summary>
    /// Правила парсинга страницы с вакансиями
    /// </summary>
    public PageWithVacanciesParseRule PageWithVacanciesParseRule { get; set; }
    
    /// <summary>
    /// Правила парсинга отдельной вакансии
    /// </summary>
    public VacancyParseRule VacancyParseRule { get; set; }
}