﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parser.Models;

/// <summary>
/// Представляет правила парсинга сайта.
/// </summary>
public class SiteParseRule
{
    /// <summary>
    /// Уникальный идентификатор правила парсинга для определенного сайта
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Название сайта, для которого установлены правила парсинга
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string SiteName { get; set; }

    /// <summary>
    /// Правила парсинга страницы с вакансиями
    /// </summary>
    [ForeignKey("PageWithVacanciesParseRuleId")]
    public virtual PageWithVacanciesParseRule PageWithVacanciesParseRule { get; set; }

    public Guid PageWithVacanciesParseRuleId { get; set; }

    /// <summary>
    /// Правила парсинга отдельной вакансии
    /// </summary>
    [ForeignKey("VacancyParseRuleId")]
    public virtual VacancyParseRule VacancyParseRule { get; set; }

    public Guid VacancyParseRuleId { get; set; }

    /// <summary>
    /// Правила парсинга страницы со списком резюме
    /// </summary>
    [ForeignKey("PageWithResumesParseRuleId")]
    public virtual PageWithResumesParseRule PageWithResumesParseRule { get; set; }

    public Guid PageWithResumesParseRuleId { get; set; }

    /// <summary>
    /// Правила парсинга отдельного резюме
    /// </summary>
    [ForeignKey("ResumeParseRuleId")]
    public virtual ResumeParseRule ResumeParseRule { get; set; }

    public Guid ResumeParseRuleId { get; set; }
}