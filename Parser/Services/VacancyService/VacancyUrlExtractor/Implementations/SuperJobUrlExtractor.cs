using System.Collections.Specialized;
using System.Text;
using System.Text.Json;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Parser.Services.VacancyParsers;

public class SuperJobUrlExtractor : DefaultVacancyUrlExtractor
{
    private Dictionary<string, int> _towns;
    private Dictionary<string, int> _regions;

    public SuperJobUrlExtractor(HttpClient httpClient) : base(httpClient)
    {
        string filePathToRegions = Path.Combine(AppContext.BaseDirectory, "Resources", "superjob_regions.json");
        string jsonContentOfRegions = System.IO.File.ReadAllText(filePathToRegions);
        JObject jsonRegions = JObject.Parse(jsonContentOfRegions);
        _regions = ExtractPlaces(jsonRegions);

        string filePathToTowns = Path.Combine(AppContext.BaseDirectory, "Resources", "superjob_towns.json");
        string jsonContentOfTowns = System.IO.File.ReadAllText(filePathToTowns);
        JObject jsonTowns = JObject.Parse(jsonContentOfTowns);
        _towns = ExtractPlaces(jsonTowns);
    }

    protected override Uri CreateLinkToStartPage(HashSet<string> keyWords, HashSet<string> places, JsonElement pageWithVacanciesParseRule)
    {
        UriBuilder startPageUrl = new UriBuilder(pageWithVacanciesParseRule.GetProperty("UrlWithVacancies").GetString());
        NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters[pageWithVacanciesParseRule.GetProperty("ParamNameForVacancyTitle").GetString()] = string.Join(",", keyWords);
        parameters[pageWithVacanciesParseRule.GetProperty("ParamNameForVacanciesWithSalary").GetString()] = "1";

        int numberOfRegions = 0;
        int numberOfTowns = 0;
        foreach (string place in places)
        {
            StringBuilder paramName = new StringBuilder(pageWithVacanciesParseRule.GetProperty("ParamNameForRegion").GetString());
            if (_regions.ContainsKey(place.ToLower()))
            {
                paramName.Append("[o]").Append("[" + numberOfRegions + "]");
                parameters[paramName.ToString()] = _regions[place.ToLower()].ToString();
                numberOfRegions++;
            }
            else if (_towns.ContainsKey(place.ToLower()))
            {
                paramName.Append("[t]").Append("[" + numberOfTowns + "]");
                parameters[paramName.ToString()] = _towns[place.ToLower()].ToString();
                numberOfTowns++;
            }
        }
        startPageUrl.Query = parameters.ToString();
        return startPageUrl.Uri;
    }

    private Dictionary<string, int> ExtractPlaces(JObject token)
    {
        Dictionary<string, int> result = new Dictionary<string, int>();
        if (token["objects"] != null)
        {
            foreach (JToken child in token["objects"])
            {
                if (token.Type == JTokenType.Object)
                {
                    var areaName = child["title"]?.ToString().ToLower();
                    var areaId = child["id"]?.ToObject<int>();

                    if (!string.IsNullOrEmpty(areaName) && areaId.HasValue)
                    {
                        result[areaName] = areaId.Value;
                    }
                }
            }
        }

        return result;
    }
}