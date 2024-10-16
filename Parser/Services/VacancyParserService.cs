using HtmlAgilityPack;
using VacancyParser.Models;

namespace VacancyParser.Services
{
    public class VacancyParserService
    {
        private readonly HttpClient _httpClient;

        public VacancyParserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Vacancy>> ParseVacanciesAsync(string url)
        {
            // Получение HTML-контента страницы
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Не удалось загрузить страницу: {response.StatusCode}");
            }

            var htmlContent = await response.Content.ReadAsStringAsync();

            // Загрузка HTML в HtmlDocument
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            // Пример: парсинг вакансий (зависит от структуры сайта)
            // Предположим, что каждая вакансия находится в <div class="vacancy-item">
            var vacancies = new List<Vacancy>();
            var vacancyNodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='vacancy-item']");
            if (vacancyNodes != null)
            {
                foreach (var node in vacancyNodes)
                {
                    var titleNode = node.SelectSingleNode(".//h2[@class='vacancy-title']/a");
                    var companyNode = node.SelectSingleNode(".//div[@class='company-name']");
                    var locationNode = node.SelectSingleNode(".//div[@class='vacancy-location']");
                    var descriptionNode = node.SelectSingleNode(".//div[@class='vacancy-description']");
                    var postedDateNode = node.SelectSingleNode(".//div[@class='posted-date']");

                    var vacancy = new Vacancy
                    {
                        Title = titleNode?.InnerText.Trim() ?? "Нет названия",
                        Company = companyNode?.InnerText.Trim() ?? "Не указана компания",
                        Location = locationNode?.InnerText.Trim() ?? "Не указано место",
                        Description = descriptionNode?.InnerText.Trim() ?? "Нет описания",
                        Url = titleNode?.GetAttributeValue("href", string.Empty) ?? string.Empty,
                        PostedDate = DateTime.TryParse(postedDateNode?.InnerText.Trim(), out var date) ? date : DateTime.MinValue
                    };

                    vacancies.Add(vacancy);
                }
            }

            return vacancies;
        }
    }
}