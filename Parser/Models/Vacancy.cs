namespace Parser.Models
{
    public class Vacancy
    {
        public string CompanyName { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string City { get; set; }
        public Uri LinkToSource { get; set; }
        public string Functional { get; set; }
        public string Requirements { get; set; }
        public string KeySkills { get; set; }
        public string Conditions { get; set; }
        public string Grade { get; set; }
        public int SalaryFrom { get; set; }
        public int SalaryTo { get; set; }

        public double getAverageSalaryValue()
        {
            return (SalaryFrom + SalaryTo) / 2.0;
        }
    }
}