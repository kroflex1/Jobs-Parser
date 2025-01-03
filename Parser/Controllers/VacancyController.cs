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
    private readonly XlsxVacancyGeneratorService _xlsxVacancyService;
    private readonly SiteParseRulesService _siteParseRuleService;
    private readonly ILogger<VacancyController> _logger;

    public VacancyController(VacanciesCollectorService vacanciesCollectorService, XlsxVacancyGeneratorService xlsxVacancyService, ILogger<VacancyController> logger,
        SiteParseRulesService siteParseRuleService)
    {
        _vacanciesCollectorService = vacanciesCollectorService;
        _xlsxVacancyService = xlsxVacancyService;
        _siteParseRuleService = siteParseRuleService;
        _logger = logger;
    }

    /// <summary>
    /// Парсит вакансии и возвращает файл XLSX с результатами.
    /// </summary>
    /// <param name="keyWords">Ключевые слова, которые должна содержать вакансия.</param>
    /// <param name="regions">Регионы, в котором ищем вакансию.</param>
    /// <param name="publicationAtMonth">Насколько давно была опубликована вакансия, 1 месяц назад, 2 месяца назад и т.д..</param>
    /// <returns>Файл XLSX с данными о вакансиях.</returns>
    [HttpGet("parse")]
    public async Task<IActionResult> ParseVacancies([FromQuery] string keyWords, [FromQuery] string regions, [FromQuery] int publicationAtMonth)
    {
        if (keyWords == null || regions == null)
        {
            return BadRequest("Список ключевых слов не может быть пустым");
        }
        if (publicationAtMonth <= 0)
        {
            return BadRequest("Количество месяцев не модет быть меньше нуля");
        }

        HashSet<string> keyWordsList = keyWords.Split(',').ToHashSet();
        HashSet<string> regionsList = regions.Split(',').Select(x => x.ToLower()).ToHashSet();
        try
        {
            List<SiteParseRule> parseRules = await _siteParseRuleService.GetAllSiteParseRules();
            List<Vacancy> vacancies = _vacanciesCollectorService.ParseVacanciesFromSites(parseRules, keyWordsList, regionsList, publicationAtMonth);
            byte[] fileBytes = _xlsxVacancyService.GenerateXlsx(vacancies);
            string fileName = $"Vacancies_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при парсинге вакансий.");
            return StatusCode(500, "Внутренняя ошибка сервера.");
        }
    }
}