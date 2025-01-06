using System.Collections.Specialized;
using System.Text;
using System.Text.Json;
using System.Web;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Parser.Utils;

namespace Parser.Services.VacancyParsers;

public class HeadHunterUrlExtractor : DefaultVacancyUrlExtractor
{
    private Dictionary<string, int> _areasMap = new Dictionary<string, int>();
    public HeadHunterUrlExtractor(HttpClient httpClient) : base(httpClient)
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
    
    protected override Uri CreateLinkToStartPage(HashSet<string> keyWords, HashSet<string> places, JsonElement pageWithVacanciesParseRule)
    {
        UriBuilder startPageUrl = new UriBuilder(pageWithVacanciesParseRule.GetProperty("UrlWithVacancies").GetString());
        NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters[pageWithVacanciesParseRule.GetProperty("ParamNameForVacancyTitle").GetString()] = string.Join(" or ", keyWords);
        parameters[pageWithVacanciesParseRule.GetProperty("ParamNameForVacanciesWithSalary").GetString()] = "true";
        StringBuilder parametersForPlaces = new StringBuilder("&");
        foreach (string region in places)
        {
            if (_areasMap.ContainsKey(region.ToLower()))
            {
                parametersForPlaces
                    .Append(pageWithVacanciesParseRule.GetProperty("ParamNameForRegion").GetString())
                    .Append("=")
                    .Append(_areasMap[region.ToLower()].ToString())
                    .Append("&");
            }
        }

        parametersForPlaces.Remove(parametersForPlaces.Length - 1, 1);
        startPageUrl.Query = parameters.ToString() + parametersForPlaces;
        return startPageUrl.Uri;
    }

    protected override List<Uri> GetVacancyUrlsFromPage(HtmlDocument htmlDocument, JsonElement pageWithVacanciesParseRule, HashSet<string> keyWords, HashSet<string> regions)
    {
        string vacancyLinkXPath = pageWithVacanciesParseRule.GetProperty("VacancyUrlNode").GetString();
        HtmlNodeCollection vacancyNodes = htmlDocument.DocumentNode.SelectNodes(vacancyLinkXPath);
        List<Uri> vacanciesLinks = new List<Uri>();

        if (vacancyNodes != null)
        {
            foreach (var node in vacancyNodes)
            {
                string vacancyLink = node.GetAttributeValue("href", string.Empty);
                if (!string.IsNullOrEmpty(vacancyLink))
                {
                    vacanciesLinks.Add(new Uri(vacancyLink));
                }
            }
        }

        return vacanciesLinks;
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