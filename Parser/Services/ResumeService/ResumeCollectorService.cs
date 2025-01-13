using Parser.Models;
using Parser.Services.ResumeService.ResumeParser;
using Parser.Services.ResumeService.ResumeParser.Implementations;
using Parser.Services.ResumeService.ResumeUrlExtractor;
using Parser.Services.ResumeService.ResumeUrlExtractor.Implementations;
using Parser.Utils;

namespace Parser.Services.ResumeService;

public class ResumeCollectorService : IResumesCollectorService
{
    private readonly Dictionary<string, IResumeParser> _resumeParsers;
    private readonly Dictionary<string, IResumeUrlExtractor> _resumeUrlExtractors;

    public ResumeCollectorService(HttpClient httpClient)
    {
        _resumeParsers = new Dictionary<string, IResumeParser>
        {
            { "default", new DefaultResumeParser(httpClient) },
            { "hh.ru", new HeadHunterResumeParser(httpClient) },
            { "superjob", new SuperJobResumeParser(httpClient) },
        };
        _resumeUrlExtractors = new Dictionary<string, IResumeUrlExtractor>
        {
            { "default", new DefaultResumeUrlExtractor(httpClient) },
            { "hh.ru", new HeadHunterResumeUrlExtractor(httpClient) },
            { "superjob", new SuperJobResumeUrlExtractor(httpClient) },
        };
    }

    public List<Resume> ParseResumesFromSites(List<SiteParseRule> siteParseRules, HashSet<string> keyWords, HashSet<string> regions)
    {
        List<Resume> result = new List<Resume>();
        foreach (SiteParseRule parseRule in siteParseRules)
        {
            String webSiteName = parseRule.SiteName;
            IResumeUrlExtractor vacancyUrlExtractor = _resumeUrlExtractors.GetValueOrDefault(webSiteName, _resumeUrlExtractors["default"]);
            IResumeParser vacancyParser = _resumeParsers.GetValueOrDefault(webSiteName, _resumeParsers["default"]);

            List<Uri> linksToVacancies =
                vacancyUrlExtractor.FindResumeUrls(keyWords, regions, TextParser.ParseStringToJsonElement(parseRule.PageWithResumesParseRule.Rules));
            List<Resume> vacancies = linksToVacancies
                .Select(link =>
                {
                    try
                    {
                        Thread.Sleep(200);
                        return vacancyParser.ParseResume(link, TextParser.ParseStringToJsonElement(parseRule.ResumeParseRule.Rules));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Что-то пошли не так", ex.Message);
                        return null;
                    }
                })
                .Where(resume => IsValidResume(resume, regions))
                .Take(30)
                .ToList();
            result.AddRange(vacancies);
        }

        return result;
    }


    private Boolean IsValidResume(Resume resume, HashSet<string> regions)
    {
        if (resume == null)
        {
            return false;
        }

        if (resume.Role == null || resume.Role.Trim().Length == 0)
        {
            return false;
        }

        return true;
    }
}