using System.Text.Json;

namespace Parser.Services.ResumeService.ResumeUrlExtractor;

public interface IResumeUrlExtractor
{
    List<Uri> FindResumeUrls(HashSet<String> keyWords, HashSet<String> regions, JsonElement pageWithResumesParseRule);
}