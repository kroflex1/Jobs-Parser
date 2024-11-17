using Microsoft.AspNetCore.Mvc;
using Parser.Data;
using Parser.Models;
using Parser.Services;

namespace Parser.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParseRulesController : ControllerBase
{
    private readonly IParseRulesService _parseRulesService;

    public ParseRulesController(IParseRulesService parseRulesService)
    {
        _parseRulesService = parseRulesService;
    }

    /// <summary>
    /// Получает настройки для парсинга вакансий сайтов.
    /// </summary>
    /// <returns>Список настроек для парсинга сайтов.</returns>
    /// <response code="200">Возвращает список настроек для парсинга страниц</response>
    /// <response code="500">Если произошла ошибка на сервере</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<SiteParseRule>>> GetAllParseRules()
    {
        List<SiteParseRule> sites = await _parseRulesService.GetAllParseRules();
        return Ok(sites);
    }

    /// <summary>
    /// Получает настройку парсинга сайта по его id.
    /// </summary>
    /// <param name="id">Id настройки парсинга.</param>
    /// <returns>Настройка парсинга сайта с указанным id.</returns>
    /// <response code="200">Возвращает найденную настройку парсинга.</response>
    /// <response code="404">Если настройка парсинга с указанным идентификатором не найдена.</response>
    [HttpGet("{id:length(24)}", Name = "GetSiteParseRuleById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SiteParseRule>> GetSiteParseRuleById(int id)
    {
        SiteParseRule siteParseRule = await _parseRulesService.GetParseRuleById(id);
        if (siteParseRule == null)
        {
            return NotFound();
        }

        return Ok(siteParseRule);
    }

    /// <summary>
    /// Обновляет настройки парсинга сайта по его id.
    /// </summary>
    /// <param name="id">Id настройки парсинга.</param>
    /// <param name="updatedSiteParseRules">Новые настройки парсинга для сайта.</param>
    /// <returns>Результат операции обновления.</returns>
    /// <response code="204">Настройки успешно обновлены.</response>
    /// <response code="404">Если настройка парсинга с указанным идентификатором не найдена.</response>
    [HttpPatch("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSiteParseRule(int id, [FromBody] SiteParseRule updatedSiteParseRules)
    {
        try
        {
            await _parseRulesService.UpdateParseRule(id, updatedSiteParseRules);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}