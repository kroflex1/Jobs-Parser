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
    /// Контакты
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
    
    public double getAverageSalaryValue()
    {
        double sum = 0;
        int amount = 0;
        if (SalaryFrom != null && SalaryFrom != 0)
        {
            sum += (int)SalaryFrom;
            amount++;
        }

        if (SalaryTo != null && SalaryTo != 0)
        {
            sum += (int)SalaryTo;
            amount++;
        }

        if (amount == 0)
        {
            return 0;
        }

        return sum / amount;
    }
}