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
            "Что будет представлять из себя стажировка?",
            "Как мы работаем",
            "Что делать",
            "Что предстоит делать"
        ]);
        XPathForRequirementTitle = TextParser.CreateXPathForNodeWithText([
            "Мы ожидаем",
            "Мы ждем от Вас",
            "Наши ожидания",
            "Требования",
            "Необходимый опыт работы и знания",
            "Требования к кандидатам",
            "Что мы ждём от специалиста",
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
            "Для нас важно",
            "Требуемая квалификация",
            "Мы будем рады видеть тебя в нашей команде, если ты имеешь",
            "Наш стэк",
            "Ты подойдёшь, если",
            "Что мы от тебя ждем"
        ]);
        XPathForConditionTitle = TextParser.CreateXPathForNodeWithText([
            "Условия",
            "Условия стажировки",
            "Условия работы",
            "С нами вам откроется возможность получить",
            "Как будет построена твоя работа",
            "Почему у нас классно работать",
            "Что ждёт тебя в Точке"
        ]);
    }

    public Vacancy? ParseVacancy(Uri linkToVacancy, JsonElement parseRules)
    {
        // Создаем HttpRequestMessage для отправки GET-запроса
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, linkToVacancy);
        foreach (var header in GetHeadersForRequest(parseRules))
        {
            request.Headers.Add(header.Key, header.Value);
        }

        HttpResponseMessage response;
        using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
        {
            try
            {
                response = _httpClient.Send(request, cts.Token);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
            }
            catch (TaskCanceledException)
            {
                // Обработка ситуации, если запрос прерван
                Console.WriteLine($"Запрос {linkToVacancy.ToString()} прерван из-за превышения времени ожидания.");
                return null;
            }
            catch (Exception ex)
            {
                // Обработка других ошибок
                Console.WriteLine($"Ошибка: {ex.Message}");
                return null;
            }
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
            CreationTime = GetCreationTime(htmlDocument, parseRules),
            WorkFormat = GetWorkFormat(htmlDocument, parseRules)
        };
        return vacancy;
    }

    protected virtual Dictionary<string, string> GetHeadersForRequest(JsonElement parseRules)
    {
        return new Dictionary<string, string>()
        {
            { "User-Agent", "JobParser" }
        };
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
        string keySkillsXPath;
        try
        {
            keySkillsXPath = parseRules.GetProperty("KeySkillsNode").GetString();
        }
        catch (Exception e)
        {
            return string.Empty;
        }

        //Получаем ключевые навыки из специального нода
        HtmlNode keySkillsNode = htmlDocument.DocumentNode.SelectSingleNode(keySkillsXPath);
        StringBuilder resultText = new StringBuilder();
        if (keySkillsNode != null)
        {
            if (keySkillsNode.Name == "ul")
            {
                foreach (HtmlNode liNode in keySkillsNode.SelectNodes(".//li"))
                {
                    resultText.Append(liNode.InnerText.Trim()).Append("\n");
                }
            }
            else
            {
                resultText.Append(keySkillsNode.InnerText.Trim());
            }
        }

        if (resultText.Length != 0)
        {
            return resultText.ToString();
        }

        //Если нод не был найден, или в нём не было информации, то получаем ключевые навыки из описания вакансии
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

        HtmlNode salaryNode = htmlDocument.DocumentNode.SelectSingleNode(salaryXPath);
        if (salaryNode != null)
        {
            string text = salaryNode.InnerText.Trim().ToLower();
            List<int> values = TextParser.ExtractNumbers(text);
            if (values.Count == 0)
            {
                return null;
            }

            if (values.Count == 2)
            {
                return values[0];
            }

            if (values.Count == 1 && text.Contains("от"))
            {
                return values[0];
            }
        }

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

        HtmlNode salaryNode = htmlDocument.DocumentNode.SelectSingleNode(salaryXPath);
        if (salaryNode != null)
        {
            string text = salaryNode.InnerText.Trim().ToLower();
            List<int> values = TextParser.ExtractNumbers(text);
            if (values.Count == 0)
            {
                return null;
            }

            if (values.Count == 2)
            {
                return values[1];
            }

            if (values.Count == 1 && text.Contains("до"))
            {
                return values[0];
            }
        }

        return null;
    }

    protected virtual List<WorkFormat>? GetWorkFormat(HtmlDocument htmlDocument, JsonElement parseRules)
    {
        string workFormatXPath;
        try
        {
            workFormatXPath = parseRules.GetProperty("WorkFormatNode").GetString();
        }
        catch (Exception e)
        {
            return new List<WorkFormat>();
        }

        HtmlNode workFormatNode = htmlDocument.DocumentNode.SelectSingleNode(workFormatXPath);
        string text = workFormatNode?.InnerText.Trim().ToLower() ?? string.Empty;
        List<WorkFormat> workFormats = new List<WorkFormat>();
        if (text.Contains("удал"))
        {
            workFormats.Add(WorkFormat.REMOTE);
        }

        if (text.Contains("гибр"))
        {
            workFormats.Add(WorkFormat.HYBRID);
        }

        if (text.Length == 0 || workFormats.Count == 0)
        {
            workFormats.Add(WorkFormat.INTERNAL);
        }

        return workFormats;
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