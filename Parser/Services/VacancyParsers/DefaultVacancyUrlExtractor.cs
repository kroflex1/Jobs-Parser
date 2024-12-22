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

    public List<Uri> FindVacancyUrls(List<string> keyWords, List<string> regions, JsonElement pageWithVacanciesParseRule)
    {
        UriBuilder startPageUrl = new UriBuilder(pageWithVacanciesParseRule.GetProperty("UrlWithVacancies").GetString());
        NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters[pageWithVacanciesParseRule.GetProperty("ParamNameForVacancyTitle").GetString()] =
            String.Format("{1} {2}", String.Join(" ", keyWords), String.Join(" ", regions));
        parameters[pageWithVacanciesParseRule.GetProperty("ParamNameForVacanciesWithSalary").GetString()] = "true";
        startPageUrl.Query = parameters.ToString();

        List<Uri> result = new List<Uri>();
        string vacancyLinkNode = pageWithVacanciesParseRule.GetProperty("VacancyUrlNode").GetString();
        string nextPageNode = pageWithVacanciesParseRule.GetProperty("VacancyUrlNode").GetString();
        Uri currentPageUrl = startPageUrl.Uri;
        while (currentPageUrl != null)
        {
            List<Uri> vacancies = GetVacancyUrlsFromPage(currentPageUrl, vacancyLinkNode);
            result.AddRange(vacancies);
            currentPageUrl = GetNextPageWithVacanciesUrl(currentPageUrl, nextPageNode);
        }

        return result;
    }

    protected virtual List<Uri> GetVacancyUrlsFromPage(Uri pageWithVacanciesUrl, string vacancyNodeLink)
    {
        string response = _httpClient.GetStringAsync(pageWithVacanciesUrl).Result;
        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(response);

        HtmlNodeCollection vacancyNodes = htmlDoc.DocumentNode.SelectNodes(vacancyNodeLink);
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

    protected virtual Uri GetNextPageWithVacanciesUrl(Uri currentPageUrl, string nextPageNodeXpath)
    {
        string response = _httpClient.GetStringAsync(currentPageUrl).Result;
        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(response);

        HtmlNode nextPageNode = htmlDoc.DocumentNode.SelectSingleNode(nextPageNodeXpath);
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