using System.Collections.Specialized;
using System.Text;
using System.Text.Json;
using System.Web;
using Newtonsoft.Json.Linq;
using Parser.Utils;

namespace Parser.Services.ResumeService.ResumeUrlExtractor.Implementations;

public class SuperJobResumeUrlExtractor : DefaultResumeUrlExtractor
{
    private Dictionary<string, int> _towns;
    private Dictionary<string, int> _regions;

    public SuperJobResumeUrlExtractor(HttpClient httpClient) : base(httpClient)
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

    protected override Uri CreateLinkToStartPage(HashSet<string> keyWords, HashSet<string> places, JsonElement pageWithResumesParseRule)
    {
        UriBuilder startPageUrl = new UriBuilder(pageWithResumesParseRule.GetProperty("UrlWithResumes").GetString());

        StringBuilder parametersForPlaces = new StringBuilder();
        int numberOfRegions = 0;
        int numberOfTowns = 0;
        foreach (string place in places)
        {
            if (_regions.ContainsKey(place.ToLower()))
            {
                parametersForPlaces.Append($"{pageWithResumesParseRule.GetProperty("ParamNameForRegion").GetString()}[{numberOfRegions}]").Append('=')
                    .Append(_regions[place.ToLower()]);
                numberOfRegions++;
                parametersForPlaces.Append("&");
            }
            else if (_towns.ContainsKey(place.ToLower()))
            {
                parametersForPlaces.Append($"{pageWithResumesParseRule.GetProperty("ParamNameForTown").GetString()}[{numberOfTowns}]").Append('=')
                    .Append(_towns[place.ToLower()]);
                numberOfTowns++;
                parametersForPlaces.Append("&");
            }
        }

        parametersForPlaces.Remove(parametersForPlaces.Length - 1, 1);

        StringBuilder parametersForKeyWords = new StringBuilder("&");
        foreach (string keyWord in keyWords)
        {
            parametersForKeyWords.Append($"{pageWithResumesParseRule.GetProperty("ParamNameForResumeTitle").GetString()}[{numberOfRegions}][keys]").Append('=')
                .Append(keyWord);
            parametersForKeyWords.Append("&");
        }

        parametersForKeyWords.Remove(parametersForKeyWords.Length - 1, 1);


        startPageUrl.Query = "?" + parametersForPlaces + parametersForKeyWords;
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