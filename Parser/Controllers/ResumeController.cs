using Microsoft.AspNetCore.Mvc;
using Parser.Models;
using Parser.Services;
using Parser.Services.ResumeService;
using Parser.Services.XlsxGenerators;

namespace Parser.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResumeController : Controller
{
    private readonly ResumeCollectorService _resumeCollectorService;
    private readonly XlsxResumeGeneratorService _xlsxResumeService;
    private readonly SiteParseRulesRepository _siteParseRuleRepository;
    private readonly ILogger<ResumeController> _logger;

    public ResumeController(ResumeCollectorService resumeCollectorService, XlsxResumeGeneratorService xlsxResumeService, ILogger<ResumeController> logger,
        SiteParseRulesRepository siteParseRuleRepository)
    {
        _resumeCollectorService = resumeCollectorService;
        _xlsxResumeService = xlsxResumeService;
        _siteParseRuleRepository = siteParseRuleRepository;
        _logger = logger;
    }

    /// <summary>
    /// Парсит резюме и возвращает файл XLSX с результатами.
    /// </summary>
    /// <param name="keyWords">Ключевые слова, которые должен содержать резюме.</param>
    /// <param name="regions">Регионы, в котором ищем резюме.</param>
    /// <returns>Файл XLSX с данными о резюме.</returns>
    [HttpGet("parse")]
    public async Task<IActionResult> ParseResumes([FromQuery] string keyWords, [FromQuery] string regions)
    {
        if (keyWords == null)
        {
            return BadRequest("Список ключевых слов не может быть пустым");
        }

        if (regions == null)
        {
            return BadRequest("Список регионов не может быть пустым");
        }

        HashSet<string> keyWordsList = keyWords.Split(',').ToHashSet();
        HashSet<string> regionsList = regions.Split(',').Select(x => x.ToLower()).ToHashSet();
        try
        {
            List<SiteParseRule> parseRules = await _siteParseRuleRepository.GetAllSiteParseRules();
            List<Resume> vacancies = _resumeCollectorService.ParseResumesFromSites(parseRules, keyWordsList, regionsList);
            byte[] fileBytes = _xlsxResumeService.GenerateXlsx(vacancies);
            string fileName = $"Resumes_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при парсинге вакансий.");
            return StatusCode(500, "Внутренняя ошибка сервера.");
        }
    }
}