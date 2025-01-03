namespace Parser.Models
{
    public class Vacancy
    {
        /// <summary>
        /// Название компании
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Название вакансии
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Город и удалёнка
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Ссылка на вакансию
        /// </summary>
        public Uri LinkToSource { get; set; }

        /// <summary>
        /// Задачи и функционал, которыми предстоит заниматься на вакансии
        /// </summary>
        public string Functional { get; set; }

        /// <summary>
        /// Требования
        /// </summary>
        public string Requirements { get; set; }

        /// <summary>
        /// Ключевые навыки
        /// </summary>
        public string KeySkills { get; set; }

        /// <summary>
        /// Условия работы
        /// </summary>
        public string Conditions { get; set; }

        /// <summary>
        /// Зарплата от
        /// </summary>
        public int? SalaryFrom { get; set; }

        /// <summary>
        /// Зарплата до
        /// </summary>
        public int? SalaryTo { get; set; }

        /// <summary>
        /// Дата публикации вакансии
        /// </summary>
        public DateTime? CreationTime { get; set; }

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
}