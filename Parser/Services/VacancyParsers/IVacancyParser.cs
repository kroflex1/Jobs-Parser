using System.Text.Json;
using Parser.Models;

namespace Parser.Services.VacancyParsers;

public interface IVacancyParser
{
    Vacancy ParseVacancy(Uri linkToVacancy, JsonElement parseRule);
}