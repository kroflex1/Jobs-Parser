using System.Collections.Specialized;
using System.Net.Http.Headers;
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
            using (var request = new HttpRequestMessage(HttpMethod.Get, currentPageUrl))
            {
                foreach (var header in GetHeadersForRequest(pageWithVacanciesParseRule))
                {
                    request.Headers.Add(header.Key, header.Value);
                }

                var requestResult = _httpClient.SendAsync(request).Result;
                if (requestResult.IsSuccessStatusCode)
                {
                    htmlDoc.LoadHtml(requestResult.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                }
                else
                {
                    return result;
                }
            }

            List<Uri> vacancies = GetVacancyUrlsFromPage(htmlDoc, pageWithVacanciesParseRule, keyWords, regions);
            result.AddRange(vacancies);
            currentPageUrl = GetNextPageWithVacanciesUrl(currentPageUrl, htmlDoc, pageWithVacanciesParseRule, keyWords, regions);
        }

        return result;
    }

    protected virtual Dictionary<string, string> GetHeadersForRequest(JsonElement pageWithVacanciesParseRule)
    {
        return new Dictionary<string, string>()
        {
            { "User-Agent", "JobParser" }
        };
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

    protected virtual List<Uri> GetVacancyUrlsFromPage(HtmlDocument htmlDocument, JsonElement pageWithVacanciesParseRule, HashSet<string> keyWords,
        HashSet<string> regions)
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
                    if (Uri.TryCreate(vacancyLink, UriKind.Absolute, out var absoluteUri))
                    {
                        vacanciesLinks.Add(new Uri(vacancyLink));
                        continue;
                    }
                    Uri baseUri = new Uri(pageWithVacanciesParseRule.GetProperty("UrlWithVacancies").GetString());
                    vacanciesLinks.Add(new Uri(baseUri, vacancyLink));
                }
            }
        }

        return vacanciesLinks;
    }

    protected virtual Uri GetNextPageWithVacanciesUrl(Uri currentPageUrl, HtmlDocument currentHtmlDocumentWithVacancies, JsonElement pageWithVacanciesParseRule,
        HashSet<string> keyWords, HashSet<string> regions)
    {
        string nextPageXpath = pageWithVacanciesParseRule.GetProperty("NextPageNode").GetString();
        HtmlNode nextPageNode = currentHtmlDocumentWithVacancies.DocumentNode.SelectSingleNode(nextPageXpath);
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