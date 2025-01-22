using Parser.Models;

namespace Parser.Services.ResumeService;

public interface IResumesCollectorService
{
    List<Resume> ParseResumesFromSites(List<SiteParseRule> siteParseRules, HashSet<string> keyWords, HashSet<string> regions, bool isKeyWordsInTitle);
}