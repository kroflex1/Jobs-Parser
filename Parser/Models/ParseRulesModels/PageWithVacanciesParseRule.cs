namespace Parser.Models;

public class PageWithVacanciesParseRule
{
    public Uri UrlWithVacancies { get; set; }
    public String UriParamForVacancyTitle { get; set; }
    public string VacancyUrlNode { get; set; }
    public string PageNumberNode { get; set; }
}