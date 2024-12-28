using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Parser.DTO;

/// <summary>
/// Представляет правила парсинга сайта.
/// </summary>
public class SiteParseRuleDTO
{
    /// <summary>
    /// Уникальный идентификатор правила парсинга для определенного сайта
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название сайта, для которого установлены правила парсинга
    /// </summary>
    public string SiteName { get; set; }

    /// <summary>
    /// Правила парсинга страницы с вакансиями
    /// </summary>
    public virtual JsonElement PageWithVacanciesParseRule { get; set; }

    /// <summary>
    /// Правила парсинга отдельной вакансии
    /// </summary>
    [ForeignKey("VacancyParseRuleId")]
    public virtual JsonElement VacancyParseRule { get; set; }
    
    /// <summary>
    /// Правила парсинга страницы со списком резюме
    /// </summary>
    [ForeignKey("PageWithResumesParseRuleId")]
    public virtual JsonElement PageWithResumesParseRule { get; set; }
    
    /// <summary>
    /// Правила парсинга отдельного резюме
    /// </summary>
    [ForeignKey("ResumeParseRuleId")]
    public virtual JsonElement ResumeParseRule { get; set; }
}