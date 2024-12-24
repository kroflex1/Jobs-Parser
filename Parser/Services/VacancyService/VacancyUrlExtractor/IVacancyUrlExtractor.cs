using System.Text.Json;

namespace Parser.Services.VacancyParsers;

public interface IVacancyUrlExtractor
{
    List<Uri> FindVacancyUrls(HashSet<String> keyWords, HashSet<String> regions, JsonElement pageWithVacanciesParseRule);
}