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
    
    public string CompanyNameNode { get; set; }
    public string NameNode { get; set; }
    public string RoleNode { get; set; }
    public string CityNode { get; set; }
    public string UrlNode { get; set; }
    public string FunctionalNode { get; set; }
    public string RequirementsNode { get; set; }
    public string KeySkillsNode { get; set; }
    public string ConditionsNode { get; set; }
    public string GradeNode { get; set; }
    public string SalaryNode { get; set; }
}