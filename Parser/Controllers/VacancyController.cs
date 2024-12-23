using Microsoft.AspNetCore.Mvc;
using Parser.Models;
using Parser.Services;
using Parser.Services.VacancyParsers;
using Parser.Services.XlsxGenerators;

namespace Parser.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VacancyController : Controller
{
    private readonly VacanciesCollectorService _vacanciesCollectorService;
    private readonly XlsxGeneratorService _xlsxService;
    private readonly SiteParseRuleService _siteParseRuleService;
    private readonly ILogger<VacancyController> _logger;

    public VacancyController(VacanciesCollectorService vacanciesCollectorService, XlsxGeneratorService xlsxService, ILogger<VacancyController> logger,
        SiteParseRuleService siteParseRuleService)
    {
        _vacanciesCollectorService = vacanciesCollectorService;
        _xlsxService = xlsxService;
        _siteParseRuleService = siteParseRuleService;
        _logger = logger;
    }

    /// <summary>
    /// Парсит вакансии с указанного URL и возвращает файл XLSX с результатами.
    /// </summary>
    /// <param name="vacanciesParameters">Параметры для поиска вакансий.</param>
    /// <returns>Файл XLSX с данными о вакансиях.</returns>
    [HttpPost("parse")]
    public async Task<IActionResult> ParseVacancies([FromBody] VacanciesParameters vacanciesParameters)
    {
        if (vacanciesParameters.keyWords == null || !vacanciesParameters.keyWords.Any())
        {
            return BadRequest("Список ключевых слов не может быть пустым");
        }

        try
        {
            List<SiteParseRule> parseRules = _siteParseRuleService.GetSiteParseRules();
            List<Vacancy> vacancies = _vacanciesCollectorService.ParseVacanciesFromSites(parseRules, vacanciesParameters.keyWords, vacanciesParameters.regions);
            byte[] fileBytes = _xlsxService.GenerateXlsx(vacancies);
            string fileName = $"Vacancies_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при парсинге вакансий.");
            return StatusCode(500, "Внутренняя ошибка сервера.");
        }
    }


    public class VacanciesParameters
    {
        public List<String> keyWords { get; set; }
        public List<String> regions { get; set; }
    }
}