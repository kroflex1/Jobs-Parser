using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using VacancyParser.Models;
using VacancyParser.Services;

namespace MyRestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VacancyController : Controller
{
    private readonly VacancyParserService _parserService;
    private readonly XlsxGeneratorService _xlsxService;
    private readonly ILogger<VacancyController> _logger;

    public VacancyController(VacancyParserService parserService, XlsxGeneratorService xlsxService, ILogger<VacancyController> logger)
    {
        _parserService = parserService;
        _xlsxService = xlsxService;
        _logger = logger;
    }

    /// <summary>
    /// Парсит вакансии с указанного URL и возвращает файл XLSX с результатами.
    /// </summary>
    /// <param name="request">Объект запроса с URL сайта для парсинга.</param>
    /// <returns>Файл XLSX с данными о вакансиях.</returns>
    [HttpPost("parse")]
    public async Task<IActionResult> ParseVacancies([FromBody] ParseRequest request)
    {
        if (string.IsNullOrEmpty(request.Url))
        {
            return BadRequest("URL не может быть пустым.");
        }

        try
        {
            // Парсинг вакансий
            var vacancies = await _parserService.ParseVacanciesAsync(request.Url);

            if (vacancies == null || !vacancies.Any())
            {
                return NotFound("Вакансии не найдены.");
            }

            // Генерация XLSX файла
            var fileBytes = _xlsxService.GenerateXlsx(vacancies);
            var fileName = $"Vacancies_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            // Возврат файла пользователю
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при парсинге вакансий.");
            return StatusCode(500, "Внутренняя ошибка сервера.");
        }
    }


    // Модель запроса
    public class ParseRequest
    {
        public string Url { get; set; }
    }
}