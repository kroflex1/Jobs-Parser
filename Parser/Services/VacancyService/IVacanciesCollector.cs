using Parser.Models;

namespace Parser.Services.VacancyParsers;

public interface IVacanciesCollector
{
    List<Vacancy> ParseVacanciesFromSites(List<SiteParseRule> siteParseRules, HashSet<string> keyWords, HashSet<string> regions);
}