using System.Collections.Specialized;
using System.Text;
using System.Text.Json;
using System.Web;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Parser.Utils;

namespace Parser.Services.VacancyParsers;

public class HeadHunterVacancyUrlExtractor : DefaultVacancyUrlExtractor
{
    private Dictionary<string, int> _areasMap = new Dictionary<string, int>();

    public HeadHunterVacancyUrlExtractor(HttpClient httpClient) : base(httpClient)
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
        parameters[pageWithVacanciesParseRule.GetProperty("ParamNameForItemsOnPage").GetString()] = "20";

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

    protected override Uri GetNextPageWithVacanciesUrl(Uri currentPageUrl, HtmlDocument currentHtmlDocumentWithVacancies,
        JsonElement pageWithVacanciesParseRule,
        HashSet<string> keyWords, HashSet<string> regions)
    {

        string paramNameForPage = pageWithVacanciesParseRule.GetProperty("ParamNameForPage").GetString();
        if (paramNameForPage != null)
        {
            NameValueCollection queryParams = HttpUtility.ParseQueryString(currentPageUrl.Query);
            // Получение значения параметра 'param1'
            string currentPageNumber = queryParams["page"];
            if (currentPageNumber == null)
            {
                queryParams[paramNameForPage] = "1";
            }
            else
            {
                int newPageNumber = int.Parse(currentPageNumber) + 1;
                queryParams[paramNameForPage] = newPageNumber.ToString();
            }
            // Строим новый Uri с измененной строкой запроса
            UriBuilder uriBuilder = new UriBuilder(currentPageUrl)
            {
                Query = queryParams.ToString()
            };
            return uriBuilder.Uri;
        }
        return null;
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