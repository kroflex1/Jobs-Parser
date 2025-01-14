using System.Text.Json;
using HtmlAgilityPack;
using Parser.Models;
using Parser.Utils;

namespace Parser.Services.ResumeService.ResumeParser.Implementations;

public class DefaultResumeParser : IResumeParser
{
    protected readonly HttpClient _httpClient;
    
    public DefaultResumeParser(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Resume? ParseResume(Uri linkToResume, JsonElement parseRules)
    {
        // Создаем HttpRequestMessage для отправки GET-запроса
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, linkToResume);
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
                Console.WriteLine($"Запрос {linkToResume.ToString()} прерван из-за превышения времени ожидания.");
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
        Resume resume = new Resume
        {
            FullName = GetFullName(htmlDocument, parseRules),
            Role = GetRole(htmlDocument, parseRules),
            Contacts = GetContacts(htmlDocument, parseRules),
            City = GetCity(htmlDocument, parseRules),
            SalaryFrom = GetSalaryFrom(htmlDocument, parseRules),
            SalaryTo = GetSalaryTo(htmlDocument, parseRules),
            LinkToSource = GetLinkToSource(htmlDocument, parseRules, linkToResume),
        };
        return resume;
    }
    
    /// <summary>
    /// Получение значений, который будут помещены в заголовки запроса.
    /// </summary>
    /// <param name="parseRules">
    /// JSON-объект с правилами парсинга, который может включать значения для заголовков.
    /// </param>
    /// <returns>
    /// Словарь, в котором key и value соответсвенно ранвы key и value для заголовка
    /// </returns>
    protected virtual Dictionary<string, string> GetHeadersForRequest(JsonElement parseRules)
    {
        return new Dictionary<string, string>()
        {
            { "User-Agent", "JobParser" }
        };
    }

    /// <summary>
    /// Извлекает ФИО рекрутера из HTML-документа с использованием настроек парсинга.
    /// </summary>
    /// <param name="htmlDocument">
    /// HTML-документ, содержащий страницу с информацией о резюме.
    /// </param>
    /// <param name="parseRules">
    /// JSON-объект с правилами парсинга, включающими путь или селектор для извлечения ФИО рекрутера.
    /// </param>
    /// <returns>
    /// ФИО рекрутера, извлечённое из HTML-документа в соответствии с заданными правилами парсинга.
    /// Если извлечь ФИО не удалось, возвращает пустю строку.
    /// </returns>
    protected virtual string GetFullName(HtmlDocument htmlDocument, JsonElement parseRules)
    {
        string fullNameXPath;
        try
        {
            fullNameXPath = parseRules.GetProperty("FullNameNode").GetString();
        }
        catch (Exception e)
        {
            return string.Empty;
        }

        HtmlNode fullNameNode = htmlDocument.DocumentNode.SelectSingleNode(fullNameXPath);
        return fullNameNode?.InnerText.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Извлекает роль/должность рекрутера из HTML-документа с использованием настроек парсинга.
    /// </summary>
    /// <param name="htmlDocument">
    /// HTML-документ, содержащий страницу с информацией о резюме.
    /// </param>
    /// <param name="parseRules">
    /// JSON-объект с правилами парсинга, включающими путь или селектор для извлечения роли/должности рекрутера.
    /// </param>
    /// <returns>
    /// роль/должность, извлечённая из HTML-документа в соответствии с заданными правилами парсинга.
    /// Если извлечь роль/должность не удалось, возвращает пустю строку.
    /// </returns>
    protected virtual string GetRole(HtmlDocument htmlDocument, JsonElement parseRules)
    {
        string roleXPath;
        try
        {
            roleXPath = parseRules.GetProperty("RoleNode").GetString();
        }
        catch (Exception e)
        {
            return string.Empty;
        }

        HtmlNode roleNode = htmlDocument.DocumentNode.SelectSingleNode(roleXPath);
        return roleNode?.InnerText.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Извлекает город проживания рекрутера из HTML-документа с использованием настроек парсинга.
    /// </summary>
    /// <param name="htmlDocument">
    /// HTML-документ, содержащий страницу с информацией о резюме.
    /// </param>
    /// <param name="parseRules">
    /// JSON-объект с правилами парсинга, включающими путь или селектор для извлечения города проживания рекрутера.
    /// </param>
    /// <returns>
    /// город проживания, извлечённый из HTML-документа в соответствии с заданными правилами парсинга.
    /// Если извлечь город проживания не удалось, возвращает пустю строку.
    /// </returns>
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
        return cityNode?.InnerText.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Извлекает контакты рекрутера из HTML-документа с использованием настроек парсинга.
    /// </summary>
    /// <param name="htmlDocument">
    /// HTML-документ, содержащий страницу с информацией о резюме.
    /// </param>
    /// <param name="parseRules">
    /// JSON-объект с правилами парсинга, включающими путь или селектор для извлечения контактов рекрутера.
    /// </param>
    /// <returns>
    /// контакты рекрутера, извлечённый из HTML-документа в соответствии с заданными правилами парсинга.
    /// Если извлечь контакты рекрутера не удалось, возвращает пустой список.
    /// </returns>
    protected virtual List<string> GetContacts(HtmlDocument htmlDocument, JsonElement parseRules)
    {
        string contactsXPath;
        try
        {
            contactsXPath = parseRules.GetProperty("ContactsNode").GetString();
        }
        catch (Exception e)
        {
            return new List<string>();
        }

        HtmlNode contactsNode = htmlDocument.DocumentNode.SelectSingleNode(contactsXPath);
        List<string> result = new List<string>();
        if (contactsNode != null)
        {
            result.Append(contactsNode.InnerText.Trim());
        }

        return result;
    }

    /// <summary>
    /// Извлекает стартовую зарплату рекрутера из HTML-документа с использованием настроек парсинга.
    /// </summary>
    /// <param name="htmlDocument">
    /// HTML-документ, содержащий страницу с информацией о резюме.
    /// </param>
    /// <param name="parseRules">
    /// JSON-объект с правилами парсинга, включающими путь или селектор для извлечения стартовой зарлаты рекрутера.
    /// </param>
    /// <returns>
    /// стартовая зарлата, извлечённая из HTML-документа в соответствии с заданными правилами парсинга.
    /// Если извлечь стартовую зарлату не удалось, возвращает null.
    /// </returns>
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
            if (values.Count == 1 && !text.Contains("до"))
            {
                return values[0];
            }
        }
        return null;
    }

    /// <summary>
    /// Извлекает стартовую зарплату рекрутера из HTML-документа с использованием настроек парсинга.
    /// </summary>
    /// <param name="htmlDocument">
    /// HTML-документ, содержащий страницу с информацией о резюме.
    /// </param>
    /// <param name="parseRules">
    /// JSON-объект с правилами парсинга, включающими путь или селектор для извлечения стартовой зарлаты рекрутера.
    /// </param>
    /// <returns>
    /// стартовая зарлата, извлечённая из HTML-документа в соответствии с заданными правилами парсинга.
    /// Если извлечь стартовую зарлату не удалось, возвращает null.
    /// </returns>
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

    protected virtual Uri GetLinkToSource(HtmlDocument htmlDocument, JsonElement parseRules, Uri linkToSource)
    {
        return linkToSource;
    }
}