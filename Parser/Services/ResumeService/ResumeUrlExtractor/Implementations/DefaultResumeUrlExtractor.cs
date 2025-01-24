using System.Collections.Specialized;
using System.Text.Json;
using System.Web;
using HtmlAgilityPack;

namespace Parser.Services.ResumeService.ResumeUrlExtractor.Implementations;

public class DefaultResumeUrlExtractor : IResumeUrlExtractor
{
    protected readonly HttpClient _httpClient;

    public DefaultResumeUrlExtractor(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public List<Uri> FindResumeUrls(HashSet<string> keyWords, HashSet<string> regions, JsonElement pageWithResumesParseRule)
    {
        Uri currentPageUrl = CreateLinkToStartPage(keyWords, regions, pageWithResumesParseRule);
        List<Uri> result = new List<Uri>();
        while (currentPageUrl != null)
        {
            Thread.Sleep(1000);
            HtmlDocument htmlDoc = new HtmlDocument();
            
            if (!currentPageUrl.ToString().StartsWith("file://"))
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, currentPageUrl))
                {
                    foreach (var header in GetHeadersForRequest(pageWithResumesParseRule))
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }

                    var requestResult = _httpClient.SendAsync(request).Result;
                    if (requestResult.IsSuccessStatusCode)
                    {
                        var x = requestResult.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        htmlDoc.LoadHtml(requestResult.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                    }
                    else
                    {
                        return result;
                    }
                }
            }
            
            List<Uri> resumeUrls = GetResumeUrlsFromPage(htmlDoc, pageWithResumesParseRule, keyWords, regions);
            result.AddRange(resumeUrls);
            currentPageUrl = GetNextPageWithResumeUrls(currentPageUrl, htmlDoc, pageWithResumesParseRule, keyWords, regions);
        }

        return result;
    }

    protected virtual Dictionary<string, string> GetHeadersForRequest(JsonElement pageWithResumesParseRule)
    {
        return new Dictionary<string, string>()
        {
            { "User-Agent", "JobParser" }
        };
    }

    protected virtual Uri CreateLinkToStartPage(HashSet<string> keyWords, HashSet<string> places, JsonElement pageWithResumesParseRule)
    {
        if (pageWithResumesParseRule.GetProperty("UrlWithResumes").GetString() == null ||
            pageWithResumesParseRule.GetProperty("UrlWithResumes").GetString().Length == 0)
        {
            return null;
        }

        UriBuilder startPageUrl = new UriBuilder(pageWithResumesParseRule.GetProperty("UrlWithResumes").GetString());
        NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters[pageWithResumesParseRule.GetProperty("ParamNameForResumeTitle").GetString()] = string.Join(" ", keyWords);
        startPageUrl.Query = parameters.ToString();
        return startPageUrl.Uri;
    }

    protected virtual List<Uri> GetResumeUrlsFromPage(HtmlDocument htmlDocument, JsonElement pageWithResumesParseRule, HashSet<string> keyWords,
        HashSet<string> regions)
    {
        string resumeLinkXPath = pageWithResumesParseRule.GetProperty("ResumeUrlNode").GetString();
        HtmlNodeCollection resumeNodes = htmlDocument.DocumentNode.SelectNodes(resumeLinkXPath);
        List<Uri> resumeLinks = new List<Uri>();

        if (resumeNodes != null)
        {
            foreach (var node in resumeNodes)
            {
                string resumeLink = node.GetAttributeValue("href", string.Empty);
                if (!string.IsNullOrEmpty(resumeLink))
                {
                    if (Uri.TryCreate(resumeLink, UriKind.Absolute, out var absoluteUri))
                    {
                        resumeLinks.Add(new Uri(resumeLink));
                        continue;
                    }
                    Uri baseUri = new Uri(pageWithResumesParseRule.GetProperty("UrlWithResumes").GetString());
                    resumeLinks.Add(new Uri(baseUri, resumeLink));
                }
            }
        }

        return resumeLinks;
    }

    protected virtual Uri GetNextPageWithResumeUrls(Uri currentPageUrl, HtmlDocument currentHtmlDocumentWithResumes, JsonElement pageWithResumesParseRule,
        HashSet<string> keyWords, HashSet<string> regions)
    {
        string nextPageXpath = pageWithResumesParseRule.GetProperty("NextPageNode").GetString();
        HtmlNode nextPageNode = currentHtmlDocumentWithResumes.DocumentNode.SelectSingleNode(nextPageXpath);
        if (nextPageNode != null)
        {
            string nextPageUrl = nextPageNode.GetAttributeValue("href", string.Empty);
            if (!string.IsNullOrEmpty(nextPageUrl))
            {
                if (Uri.TryCreate(nextPageUrl, UriKind.Absolute, out var absoluteUri))
                {
                    return new Uri(nextPageUrl);
                }

                Uri baseUri = new Uri(pageWithResumesParseRule.GetProperty("UrlWithResumes").GetString());
                baseUri = new Uri(baseUri.Host);
                return new Uri(baseUri, nextPageUrl);
            }
        }

        return null;
    }
}