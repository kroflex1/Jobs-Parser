namespace Parser.Models;

/// <summary>
/// Представляет правила для парсинга страницы с вакансиями.
/// </summary>
public class PageWithVacanciesParseRule
{
    /// <summary>
    /// URL-адрес страницы с вакансиями
    /// </summary>
    public Uri UrlWithVacancies { get; set; }

    /// <summary>
    /// Параметр запроса для нахождения заголовка вакансии
    /// </summary>
    public string ParamNameForVacancyTitle { get; set; }

    /// <summary>
    /// Нода для ссылки на вакансию
    /// </summary>
    public string VacancyUrlNode { get; set; }

    /// <summary>
    /// Нода для номера страницы
    /// </summary>
    public string PageNumberNode { get; set; }
}