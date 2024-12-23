using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parser.Models;

/// <summary>
/// Представляет правила для парсинга страницы с вакансиями.
/// </summary>
public class PageWithVacanciesParseRule
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    /// <summary>
    /// Правила парсинга страницы в формате json
    /// </summary>
    public String Rules { get; set; }
}