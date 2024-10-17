using HtmlAgilityPack;
using Parser.Models;

namespace Parser.Services.VacancyParsers
{
    public class VacancyParserService : IVacancyParser
    {
        private readonly HttpClient _httpClient;

        public VacancyParserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Vacancy ParseVacancy(string url, VacancyParsingSetting parsingSetting)
        {
            // Создаем HttpRequestMessage для отправки GET-запроса
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Выполняем синхронный запрос
            var response = _httpClient.Send(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Не удалось загрузить страницу: {response.StatusCode}");
            }

            // Получаем HTML-контент страницы
            var htmlContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            // Загрузка HTML в HtmlDocument
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            var companyNameNode = htmlDoc.DocumentNode.SelectSingleNode(parsingSetting.CompanyNameNode);
            var nameNode = htmlDoc.DocumentNode.SelectSingleNode(parsingSetting.NameNode);
            var roleNode = htmlDoc.DocumentNode.SelectSingleNode(parsingSetting.RoleNode);
            var cityNode = htmlDoc.DocumentNode.SelectSingleNode(parsingSetting.CityNode);
            var functionalNode = htmlDoc.DocumentNode.SelectSingleNode(parsingSetting.FunctionalNode);
            var requirementsNode = htmlDoc.DocumentNode.SelectSingleNode(parsingSetting.RequirementsNode);
            var keySkillsNode = htmlDoc.DocumentNode.SelectSingleNode(parsingSetting.KeySkillsNode);
            var conditionsNode = htmlDoc.DocumentNode.SelectSingleNode(parsingSetting.ConditionsNode);
            var gradeNode = htmlDoc.DocumentNode.SelectSingleNode(parsingSetting.GradeNode);

            var vacancy = new Vacancy
            {
                CompanyName = companyNameNode?.InnerText.Trim() ?? string.Empty,
                Name = nameNode?.InnerText.Trim() ?? string.Empty,
                Role = roleNode?.InnerText.Trim() ?? string.Empty,
                City = cityNode?.InnerText.Trim() ?? string.Empty,
                Functional = functionalNode?.InnerText.Trim() ?? string.Empty,
                Requirements = requirementsNode?.InnerText.Trim() ?? string.Empty,
                KeySkills = keySkillsNode?.InnerText.Trim() ?? string.Empty,
                Conditions = conditionsNode?.InnerText.Trim() ?? string.Empty,
                Grade = gradeNode?.InnerText.Trim() ?? string.Empty
            };
            return vacancy;
        }
    }
}