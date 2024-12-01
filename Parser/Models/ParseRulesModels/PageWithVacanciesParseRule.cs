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
    /// URL-адрес страницы с вакансиями
    /// </summary>
    [Required]
    public String UrlWithVacancies { get; set; }

    /// <summary>
    /// Параметр запроса для нахождения заголовка вакансии
    /// </summary>
    public string ParamNameForVacancyTitle { get; set; }
    
    /// <summary>
    /// Параметр запроса для нахождения вакансий только с зарплатой
    /// </summary>
    public string ParamNameForVacanciesWithSalary { get; set; }

    /// <summary>
    /// Нода для ссылки на вакансию
    /// </summary>
    public string VacancyUrlNode { get; set; }

    /// <summary>
    /// Нода для номера страницы
    /// </summary>
    public string PageNumberNode { get; set; }
}