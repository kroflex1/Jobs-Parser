using Parser.Models;

namespace Parser.Services.VacancyParsers;

public interface IVacancyParser
{
    Vacancy ParseVacancy(string vacancyUrl, VacancyParsingSetting parsingSetting);
}