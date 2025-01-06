using System.Collections.Specialized;
using System.Text.Json;
using System.Web;
using HtmlAgilityPack;

namespace Parser.Services.VacancyParsers;

public class DefaultVacancyUrlExtractor : IVacancyUrlExtractor
{
    protected readonly HttpClient _httpClient;

    public DefaultVacancyUrlExtractor(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public List<Uri> FindVacancyUrls(HashSet<string> keyWords, HashSet<string> regions, JsonElement pageWithVacanciesParseRule)
    {
        Uri currentPageUrl = CreateLinkToStartPage(keyWords, regions, pageWithVacanciesParseRule);
        List<Uri> result = new List<Uri>();
        while (currentPageUrl != null)
        {
            Thread.Sleep(1000);
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(_httpClient.GetStringAsync(currentPageUrl).Result);

            List<Uri> vacancies = GetVacancyUrlsFromPage(htmlDoc, pageWithVacanciesParseRule, keyWords, regions);
            result.AddRange(vacancies);
            currentPageUrl = GetNextPageWithVacanciesUrl(currentPageUrl, htmlDoc, pageWithVacanciesParseRule, keyWords, regions);
        }
        return result;
    }

    protected virtual Uri CreateLinkToStartPage(HashSet<string> keyWords, HashSet<string> places, JsonElement pageWithVacanciesParseRule)
    {
        UriBuilder startPageUrl = new UriBuilder(pageWithVacanciesParseRule.GetProperty("UrlWithVacancies").GetString());
        NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters[pageWithVacanciesParseRule.GetProperty("ParamNameForVacancyTitle").GetString()] = string.Join(" ", keyWords);
        parameters[pageWithVacanciesParseRule.GetProperty("ParamNameForVacanciesWithSalary").GetString()] = "true";
        startPageUrl.Query = parameters.ToString();
        return startPageUrl.Uri;
    }

    protected virtual List<Uri> GetVacancyUrlsFromPage(HtmlDocument htmlDocument, JsonElement pageWithVacanciesParseRule, HashSet<string> keyWords, HashSet<string> regions)
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

    protected virtual Uri GetNextPageWithVacanciesUrl(Uri currentPageUrl, HtmlDocument htmlDocument, JsonElement pageWithVacanciesParseRule, HashSet<string> keyWords, HashSet<string> regions)
    {
        string response = _httpClient.GetStringAsync(currentPageUrl).Result;
        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(response);

        string nextPageXpath = pageWithVacanciesParseRule.GetProperty("NextPageNode").GetString();
        HtmlNode nextPageNode = htmlDoc.DocumentNode.SelectSingleNode(nextPageXpath);
        if (nextPageNode != null)
        {
            string nextPageUrl = nextPageNode.GetAttributeValue("href", string.Empty);
            if (!string.IsNullOrEmpty(nextPageUrl))
            {
                if (Uri.TryCreate(nextPageUrl, UriKind.Absolute, out var absoluteUri))
                {
                    return new Uri(nextPageUrl);
                }

                Uri baseUri = currentPageUrl;
                return new Uri(baseUri, nextPageUrl);
            }
        }

        return null;
    }
}