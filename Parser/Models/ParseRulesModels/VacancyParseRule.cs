using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parser.Models;

/// <summary>
/// Представляет правила для парсинга информации о вакансии.
/// </summary>
public class VacancyParseRule
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Нода для получения названия компании
    /// </summary>
    public string CompanyNameNode { get; set; }

    /// <summary>
    /// Нода для получения названия вакансии
    /// </summary>
    public string NameNode { get; set; }

    /// <summary>
    /// Нода для получения...
    /// </summary>
    public string RoleNode { get; set; }

    /// <summary>
    /// Правила для получения места работы
    /// </summary>
    public string CityNode { get; set; }

    /// <summary>
    /// Нодя для получения описания вакансии
    /// </summary>
    public string DescriptionNode { get; set; }

    /// <summary>
    /// Нода для получения грейда сотрудника
    /// </summary>
    public string GradeNode { get; set; }

    /// <summary>
    /// Нода для получения зарплаты
    /// </summary>
    public string SalaryNode { get; set; }
}