using Parser.Models;

namespace Parser.Services.VacancyParsers;

public interface IVacancyParser
{
    List<Vacancy> ParseVacanciesFromSites(List<SiteParseRule> siteParseRules, String keyWord, String region);
}