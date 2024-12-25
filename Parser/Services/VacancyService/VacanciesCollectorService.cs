using Parser.Models;
using Parser.Utils;

namespace Parser.Services.VacancyParsers;

public class VacanciesCollectorService : IVacanciesCollector
{
    private readonly Dictionary<string, IVacancyParser> _vacancyParsers;
    private readonly Dictionary<string, IVacancyUrlExtractor> _vacancyUrlExtractors;

    public VacanciesCollectorService(HttpClient httpClient)
    {
        _vacancyParsers = new Dictionary<string, IVacancyParser>
        {
            { "default", new DefaultVacancyParser(httpClient) },
            { "hh.ru", new HeadHunterVacancyParser(httpClient) },
        };
        _vacancyUrlExtractors = new Dictionary<string, IVacancyUrlExtractor>
        {
            { "default", new DefaultVacancyUrlExtractor(httpClient) },
            { "hh.ru", new HeadHunterUrlExtractor(httpClient) },
        };
    }

    public List<Vacancy> ParseVacanciesFromSites(List<SiteParseRule> siteParseRules, HashSet<string> keyWords, HashSet<string> regions)
    {
        List<Vacancy> result = new List<Vacancy>();
        foreach (SiteParseRule parseRule in siteParseRules)
        {
            String webSiteName = parseRule.SiteName;
            IVacancyUrlExtractor vacancyUrlExtractor = _vacancyUrlExtractors.GetValueOrDefault(webSiteName, _vacancyUrlExtractors["default"]);
            IVacancyParser vacancyParser = _vacancyParsers.GetValueOrDefault(webSiteName, _vacancyParsers["default"]);

            List<Uri> linksToVacancies =
                vacancyUrlExtractor.FindVacancyUrls(keyWords, regions, TextParser.ParseStringToJsonElement(parseRule.PageWithVacanciesParseRule.Rules));
            List<Vacancy> vacancies = linksToVacancies
                .Select(link =>
                {
                    Thread.Sleep(1000);
                    return vacancyParser.ParseVacancy(link, TextParser.ParseStringToJsonElement(parseRule.VacancyParseRule.Rules));
                })
                .Where(vacancy => IsValidVacancy(vacancy, regions))
                .ToList();
            result.AddRange(vacancies);
        }

        return result;
    }


    private Boolean IsValidVacancy(Vacancy vacancy, HashSet<string> regions)
    {
        if (vacancy == null)
        {
            return false;
        }

        if (vacancy.SalaryFrom == null && vacancy.SalaryTo == null)
        {
            return false;
        }

        if (vacancy.CreationTime == null)
        {
            return false;
        }

        if (vacancy.City == null || vacancy.City.Length == 0)
        {
            return false;
        }

        return true;
    }
}