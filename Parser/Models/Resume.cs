namespace Parser.Models;

public class Resume
{
    /// <summary>
    /// ФИО
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Роль/должность
    /// </summary>
    public string Role { get; set; }

    /// <summary>
    /// Роль/должность
    /// </summary>
    public List<string> Contacts { get; set; }

    /// <summary>
    /// Город проживания
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// Зарплата от
    /// </summary>
    public int? SalaryFrom { get; set; }

    /// <summary>
    /// Зарплата до
    /// </summary>
    public int? SalaryTo { get; set; }

    /// <summary>
    /// Ссылка на резюме
    /// </summary>
    public Uri LinkToSource { get; set; }
}