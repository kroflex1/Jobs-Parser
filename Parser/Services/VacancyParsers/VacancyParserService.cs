using System.Collections.Specialized;
using System.Web;
using HtmlAgilityPack;
using Parser.Models;

namespace Parser.Services.VacancyParsers;

public class VacancyParserService : IVacancyParser
{
    private readonly HttpClient _httpClient;

    public VacancyParserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public List<Vacancy> ParseVacanciesFromSites(List<SiteParseRule> siteParseRules, string keyWord, string region)
    {
        List<Vacancy> result = new List<Vacancy>();
        foreach (SiteParseRule parseRule in siteParseRules)
        {
            result.AddRange(ParseVacanciesFromSite(parseRule, keyWord, region));
        }
        return result;
    }

    public List<Vacancy> ParseVacanciesFromSite(SiteParseRule siteParseRule, string keyWord, string region)
    {
        UriBuilder startPageUrl = new UriBuilder(siteParseRule.PageWithVacanciesParseRule.UrlWithVacancies);
        NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters[siteParseRule.PageWithVacanciesParseRule.ParamNameForVacancyTitle] = String.Format("{1} {2}", keyWord, region);
        parameters[siteParseRule.PageWithVacanciesParseRule.ParamNameForVacanciesWithSalary] = "true";
        startPageUrl.Query = parameters.ToString();

        List<Uri> vacancyUrls = GetAllVacancyUrls(startPageUrl.Uri, siteParseRule.PageWithVacanciesParseRule.VacancyUrlNode,
            siteParseRule.PageWithVacanciesParseRule.NextPageNode);
        List<Vacancy> vacancies = new List<Vacancy>();
        foreach (Uri vacancyUrl in vacancyUrls)
        {
            vacancies.Add(ParseVacancy(vacancyUrl, siteParseRule.VacancyParseRule));
        }
                
        return new List<Vacancy>();
    }
        
    public Vacancy ParseVacancy(Uri linkToVacancy, VacancyParseRule parseRule)
    {
        // Создаем HttpRequestMessage для отправки GET-запроса
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, linkToVacancy);

        // Выполняем синхронный запрос
        HttpResponseMessage response = _httpClient.Send(request);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        // Получаем HTML-контент страницы
        string htmlContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        // Загрузка HTML в HtmlDocument
        HtmlDocument htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        HtmlNode companyNameNode = htmlDoc.DocumentNode.SelectSingleNode(parseRule.CompanyNameNode);
        HtmlNode nameNode = htmlDoc.DocumentNode.SelectSingleNode(parseRule.NameNode);
        HtmlNode cityNode = htmlDoc.DocumentNode.SelectSingleNode(parseRule.CityNode);
        HtmlNode descriptionNode = htmlDoc.DocumentNode.SelectSingleNode(parseRule.DescriptionNode);
        HtmlNode salaryNode = htmlDoc.DocumentNode.SelectSingleNode(parseRule.SalaryNode);
        HtmlNode creationTimeNode = htmlDoc.DocumentNode.SelectSingleNode(parseRule.SalaryNode);

        Vacancy vacancy = new Vacancy
        {
            CompanyName = companyNameNode?.InnerText.Trim() ?? string.Empty,
            Name = nameNode?.InnerText.Trim() ?? string.Empty,
            Role = "",
            City = cityNode?.InnerText.Trim() ?? string.Empty,
            Functional = "",
            Requirements = "",
            KeySkills = "",
            Conditions = "",
            Grade = "",
            SalaryFrom = 0,
            SalaryTo = 0,
            CreationTime = DateTime.Now
        };
        return vacancy;
    }
        
    public List<Uri> GetAllVacancyUrls(Uri startPageUrl, string vacancyLinkNode, string nextPageNode)
    {
        List<Uri> result = new List<Uri>();
        Uri currentPageUrl = startPageUrl;
        while (currentPageUrl != null)
        {
            List<Uri> vacancies = GetVacancyUrlsFromPage(currentPageUrl, vacancyLinkNode);
            result.AddRange(vacancies);
            currentPageUrl = GetNextPageWithVacanciesUrl(currentPageUrl, nextPageNode);
        }
        return result;
    }

    public List<Uri> GetVacancyUrlsFromPage(Uri pageWithVacanciesUrl, string vacancyNodeLink)
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

    public Uri GetNextPageWithVacanciesUrl(Uri currentPageUrl, string nextPageNodeXpath)
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