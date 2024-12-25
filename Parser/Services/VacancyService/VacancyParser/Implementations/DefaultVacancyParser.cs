using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Parser.Models;
using Parser.Utils;

namespace Parser.Services.VacancyParsers;

public class DefaultVacancyParser : IVacancyParser
{
    protected readonly HttpClient _httpClient;
    protected readonly String XPathForFunctionalTitle;
    protected readonly String XPathForRequirementTitle;
    protected readonly String XPathForConditionTitle;


    public DefaultVacancyParser(HttpClient httpClient)
    {
        _httpClient = httpClient;
        XPathForFunctionalTitle = TextParser.CreateXPathForNodeWithText([
            "Что предлагаем",
            "Чем предстоит заниматься",
            "Наши задачи",
            "Что мы предлагаем",
            "Какие задачи вас ждут",
            "Основные задачи",
            "Обязанности",
            "Основными задачами будут",
            "О задачах",
            "Вам предстоит",
            "Какие задачи ждут тебя",
            "Задачи",
            "Над чем предстоит работать",
            "Что будет представлять из себя стажировка?"
        ]);
        XPathForRequirementTitle = TextParser.CreateXPathForNodeWithText([
            "Мы ожидаем",
            "Мы ждем от Вас",
            "Наши ожидания",
            "Требования",
            "Необходимый опыт работы и знания",
            "Требования к кандидатам",
            "Что мы ждём от специалиста",
            "Требования",
            "Пожелания к кандидату",
            "Что мы ожидаем от тебя",
            "Минимальные требования",
            "Мы ждем, что ты",
            "Мы ждем, что вы",
            "Необходимые компетенции",
            "В процессе работы ты будешь",
            "Мы ожидаем от кандидата",
            "Чтобы справляться с задачами нужны",
            "Нам нужен человек, который",
            "Для нас важно"
        ]);
        XPathForConditionTitle = TextParser.CreateXPathForNodeWithText([
            "Условия",
            "Условия стажировки",
            "Условия работы",
            "С нами вам откроется возможность получить"
        ]);
    }

    public Vacancy? ParseVacancy(Uri linkToVacancy, JsonElement parseRules)
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
        HtmlDocument htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(htmlContent);
        Vacancy vacancy = new Vacancy
        {
            CompanyName = GetCompanyName(htmlDocument, parseRules),
            Name = GetVacancyName(htmlDocument, parseRules),
            City = GetCity(htmlDocument, parseRules),
            LinkToSource = GetLinkToSource(htmlDocument, parseRules, linkToVacancy),
            Functional = GetFunctional(htmlDocument, parseRules),
            Requirements = GetRequirements(htmlDocument, parseRules),
            KeySkills = GetKeySkills(htmlDocument, parseRules),
            Conditions = GetConditions(htmlDocument, parseRules),
            SalaryFrom = GetSalaryFrom(htmlDocument, parseRules),
            SalaryTo = GetSalaryTo(htmlDocument, parseRules),
            CreationTime = GetCreationTime(htmlDocument, parseRules)
        };
        return vacancy;
    }

    /// <summary>
    /// Извлекает название компании из HTML-документа с использованием настроек парсинга.
    /// </summary>
    /// <param name="htmlDocument">
    /// HTML-документ, содержащий страницу с информацией о компании.
    /// </param>
    /// <param name="parseRules">
    /// JSON-объект с правилами парсинга, включающими путь или селектор для извлечения названия компании.
    /// </param>
    /// <returns>
    /// Название компании, извлечённое из HTML-документа в соответствии с заданными правилами парсинга.
    /// Если извлечь название компании не удалось, возвращает пустю строку.
    /// </returns>
    protected virtual string GetCompanyName(HtmlDocument htmlDocument, JsonElement parseRules)
    {
        string companyNameXPath;
        try
        {
            companyNameXPath = parseRules.GetProperty("CompanyNameNode").GetString();
        }
        catch (Exception e)
        {
            return string.Empty;
        }

        HtmlNode companyNameNode = htmlDocument.DocumentNode.SelectSingleNode(companyNameXPath);
        return companyNameNode?.InnerText.Trim() ?? string.Empty;
    }

    protected virtual string GetVacancyName(HtmlDocument htmlDocument, JsonElement parseRules)
    {
        string vacancyNameXPath;
        try
        {
            vacancyNameXPath = parseRules.GetProperty("NameNode").GetString();
        }
        catch (Exception e)
        {
            return string.Empty;
        }

        HtmlNode vacancyNameNode = htmlDocument.DocumentNode.SelectSingleNode(vacancyNameXPath);
        return vacancyNameNode?.InnerText.Trim() ?? string.Empty;
    }


    protected virtual string GetCity(HtmlDocument htmlDocument, JsonElement parseRules)
    {
        string cityXPath;
        try
        {
            cityXPath = parseRules.GetProperty("CityNode").GetString();
        }
        catch (Exception e)
        {
            return string.Empty;
        }

        HtmlNode cityNode = htmlDocument.DocumentNode.SelectSingleNode(cityXPath);
        if (cityNode == null)
        {
            return String.Empty;
        }
        List<string> parts = cityNode.InnerText.Split(',').ToList();
        if (parts.Count == 0)
        {
            return cityNode.InnerText.Trim();
        }

        return parts[0].Trim();
    }

    protected virtual Uri GetLinkToSource(HtmlDocument htmlDocument, JsonElement parseRules, Uri linkToVacancy)
    {
        return linkToVacancy;
    }

    /// <summary>
    /// Извлекает Задачи и функционал, которыми предстоит заниматься на обязанности из HTML-документа с использованием переданных настроек парсинга.
    /// </summary>
    /// <param name="htmlDocument">
    /// HTML-документ, содержащий страницу с описанием функциональных обязанностей.
    /// </param>
    /// <param name="parseRules">
    /// JSON-объект с правилами парсинга, включающими путь или селектор для извлечения функциональных обязанностей.
    /// </param>
    /// <returns>
    /// Функциональные обязанности, извлечённые из HTML-документа в соответствии с заданными правилами парсинга.
    /// По умолчанию возвращает пустую строку. Если данные не найдены возвращает пустую строку.
    /// </returns>
    protected virtual string GetFunctional(HtmlDocument htmlDocument, JsonElement parseRules)
    {
        return String.Empty;
    }

    /// <summary>
    /// Извлекает требования для вакансии из HTML-документа с использованием переданных настроек парсинга.
    /// </summary>
    /// <param name="htmlDocument">
    /// HTML-документ, содержащий страницу с описанием требований к кандидату.
    /// </param>
    /// <param name="parseRules">
    /// JSON-объект с правилами парсинга, включающими путь или селектор для извлечения требований.
    /// </param>
    /// <returns>
    /// Требования и ожидаемые навыки, извлечённые из HTML-документа в соответствии с заданными правилами парсинга.
    /// По умолчанию возвращает пустую строку. Если данные не найдены возвращает пустую строку.
    /// </returns>
    protected virtual string GetRequirements(HtmlDocument htmlDocument, JsonElement parseRules)
    {
        return String.Empty;
    }

    protected virtual string GetKeySkills(HtmlDocument htmlDocument, JsonElement parseRules)
    {
        string requirements = GetRequirements(htmlDocument, parseRules);

        // Регулярное выражение для поиска английских слов
        string pattern = @"\b[A-Za-z]+(?:[-/][A-Za-z]+)*\b";
        MatchCollection matches = Regex.Matches(requirements, pattern);
        
        StringBuilder result = new StringBuilder();
        foreach (Match match in matches)
        {
            result.Append(match.Value).Append('\n');
        }

        if (result.Length > 0)
        {
            result.Remove(result.Length - 1, 1);
        }
        return result.ToString();
    }

    /// <summary>
    /// Извлекает информацию о услових работы по вакансии из HTML-документа с использованием переданных настроек парсинга.
    /// </summary>
    /// <param name="htmlDocument">
    /// HTML-документ, содержащий страницу с описанием условий работы.
    /// </param>
    /// <param name="parseRules">
    /// JSON-объект с правилами парсинга, включающими путь или селектор для извлечения условий работы.
    /// </param>
    /// <returns>
    /// Условия работы, извлечённые из HTML-документа в соответствии с заданными правилами парсинга.
    /// По умолчанию возвращает пустую строку. Если данные не найдены возвращает пустую строку.
    /// </returns>
    protected virtual string GetConditions(HtmlDocument htmlDocument, JsonElement parseRules)
    {
        return String.Empty;
    }

    protected virtual int? GetSalaryFrom(HtmlDocument htmlDocument, JsonElement parseRules)
    {
        string salaryXPath;
        try
        {
            salaryXPath = parseRules.GetProperty("SalaryNode").GetString();
        }
        catch (Exception e)
        {
            return null;
        }

        HtmlNode cityNode = htmlDocument.DocumentNode.SelectSingleNode(salaryXPath);
        if (cityNode != null)
            return TextParser.ExtractNumbers(cityNode.InnerText.Trim()).Item1;
        return null;
    }

    protected virtual int? GetSalaryTo(HtmlDocument htmlDocument, JsonElement parseRules)
    {
        string salaryXPath;
        try
        {
            salaryXPath = parseRules.GetProperty("SalaryNode").GetString();
        }
        catch (Exception e)
        {
            return null;
        }

        HtmlNode cityNode = htmlDocument.DocumentNode.SelectSingleNode(salaryXPath);
        if (cityNode != null)
            return TextParser.ExtractNumbers(cityNode.InnerText.Trim()).Item2;
        return null;
    }

    protected virtual DateTime? GetCreationTime(HtmlDocument htmlDocument, JsonElement parseRules)
    {
        string creationTimeXPath;
        try
        {
            creationTimeXPath = parseRules.GetProperty("CreationTimeNode").GetString();
        }
        catch (Exception e)
        {
            return null;
        }

        HtmlNode creationTimeNode = htmlDocument.DocumentNode.SelectSingleNode(creationTimeXPath);
        String dateText = creationTimeNode?.InnerText.Trim() ?? string.Empty;
        return TextParser.ParseDate(dateText);
    }
}