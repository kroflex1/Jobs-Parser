using Parser.Models;

namespace Parser.Services.VacancyParsers;

public interface IVacanciesCollector
{
    List<Vacancy> ParseVacanciesFromSites(List<SiteParseRule> siteParseRules, List<string> keyWords, List<string> regions);
}