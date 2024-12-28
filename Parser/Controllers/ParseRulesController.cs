using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Parser.Data;
using Parser.DTO;
using Parser.Models;
using Parser.Services;

namespace Parser.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParseRulesController : ControllerBase
{
    private readonly ISiteParseRulesService _siteParseRulesService;

    public ParseRulesController(ISiteParseRulesService siteParseRulesService)
    {
        _siteParseRulesService = siteParseRulesService;
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
    public async Task<ActionResult<List<SiteParseRuleDTO>>> GetAllParseRules()
    {
        List<SiteParseRule> sites = await _siteParseRulesService.GetAllSiteParseRules();
        
        return Ok(sites.Select(convertEntityToDTO).ToList());
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
    public async Task<ActionResult<SiteParseRuleDTO>> GetSiteParseRuleById(Guid id)
    {
        SiteParseRule siteParseRule = await _siteParseRulesService.GetSiteParseRuleById(id);

        return Ok(convertEntityToDTO(siteParseRule));
    }

    /// <summary>
    /// Обновляет настройки парсинга сайта по его id.
    /// </summary>
    /// <param name="updatedSiteParseRules">Новые настройки парсинга для сайта.</param>
    /// <returns>Результат операции обновления.</returns>
    /// <response code="204">Настройки успешно обновлены.</response>
    /// <response code="404">Если настройка парсинга с указанным идентификатором не найдена.</response>
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSiteParseRule([FromBody] SiteParseRuleDTO updatedSiteParseRules)
    {
        try
        {
            await _siteParseRulesService.UpdateSiteParseRule(updatedSiteParseRules);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    private SiteParseRuleDTO convertEntityToDTO(SiteParseRule siteParseRule)
    {
        return new SiteParseRuleDTO
        {
            Id = siteParseRule.Id,
            SiteName = siteParseRule.SiteName,
            PageWithVacanciesParseRule = JsonSerializer.Deserialize<JsonElement>(siteParseRule.PageWithVacanciesParseRule.Rules),
            VacancyParseRule = JsonSerializer.Deserialize<JsonElement>(siteParseRule.VacancyParseRule.Rules),
            PageWithResumesParseRule = JsonSerializer.Deserialize<JsonElement>(siteParseRule.PageWithResumesParseRule.Rules),
            ResumeParseRule = JsonSerializer.Deserialize<JsonElement>(siteParseRule.ResumeParseRule.Rules)
        };
    }
}