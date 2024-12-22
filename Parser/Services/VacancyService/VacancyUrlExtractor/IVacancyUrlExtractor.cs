using System.Text.Json;

namespace Parser.Services.VacancyParsers;

public interface IVacancyUrlExtractor
{
    List<Uri> FindVacancyUrls(List<String> keyWords, List<String> regions, JsonElement pageWithVacanciesParseRule);
}