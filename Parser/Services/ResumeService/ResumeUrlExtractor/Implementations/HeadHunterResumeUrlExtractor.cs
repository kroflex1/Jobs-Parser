using System.Collections.Specialized;
using System.Text;
using System.Text.Json;
using System.Web;
using Newtonsoft.Json.Linq;
using Parser.Utils;

namespace Parser.Services.ResumeService.ResumeUrlExtractor.Implementations;

public class HeadHunterResumeUrlExtractor : DefaultResumeUrlExtractor
{
    private Dictionary<string, int> _areasMap = new Dictionary<string, int>();

    public HeadHunterResumeUrlExtractor(HttpClient httpClient) : base(httpClient)
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, "Resources", "hh_regions.json");
        string jsonContent = System.IO.File.ReadAllText(filePath);
        JObject jsonData = JObject.Parse(jsonContent);

        if (jsonData["areas"] != null)
        {
            foreach (var area in jsonData["areas"])
            {
                ExtractAreas(area);
            }
        }
    }

    protected override Uri CreateLinkToStartPage(HashSet<string> keyWords, HashSet<string> places, JsonElement pageWithResumesParseRule)
    {
        UriBuilder startPageUrl = new UriBuilder(pageWithResumesParseRule.GetProperty("UrlWithResumes").GetString());
        NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters[pageWithResumesParseRule.GetProperty("ParamNameForResumeTitle").GetString()] = string.Join(" or ", keyWords);
        parameters["pos"] = "full_text";
        parameters["logic"] = "normal";
        parameters["exp_period"] = "all_time";
        
        List<string> valuesForSearchStatus = new List<string>() { "active_search", "looking_for_offers" };
        String searchStatusParameter =
            TextParser.CreateParameters(pageWithResumesParseRule.GetProperty("ParamNameForSearchStatus").GetString(), valuesForSearchStatus);

        List<string> valuesForPlaces = places
            .Where(place => _areasMap.ContainsKey(place.ToLower()))
            .Select(place=>_areasMap[place.ToLower()].ToString())
            .ToList();
        String placesParameter =
            TextParser.CreateParameters(pageWithResumesParseRule.GetProperty("ParamNameForRegion").GetString(), valuesForPlaces);
        
        
        startPageUrl.Query = parameters + placesParameter + searchStatusParameter;
        return startPageUrl.Uri;
    }

    // Метод извлечения данных из "areas"
    private void ExtractAreas(JToken token)
    {
        if (token == null) return;

        // Проверка, является ли текущий объект областью
        if (token.Type == JTokenType.Object)
        {
            var areaName = token["name"]?.ToString().ToLower();
            var areaId = token["id"]?.ToObject<int>();

            if (!string.IsNullOrEmpty(areaName) && areaId.HasValue)
            {
                _areasMap[areaName] = areaId.Value;
            }
        }

        // Рекурсивная обработка вложенных объектов или массивов
        if (token["areas"] != null)
        {
            foreach (var child in token["areas"])
            {
                ExtractAreas(child);
            }
        }
    }
}