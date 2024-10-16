using ClosedXML.Excel;
using VacancyParser.Models;

namespace VacancyParser.Services
{
    public class XlsxGeneratorService
    {
        public byte[] GenerateXlsx(List<Vacancy> vacancies)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Vacancies");

                // Добавление заголовков
                worksheet.Cell(1, 1).Value = "Название";
                worksheet.Cell(1, 2).Value = "Компания";
                worksheet.Cell(1, 3).Value = "Местоположение";
                worksheet.Cell(1, 4).Value = "Описание";
                worksheet.Cell(1, 5).Value = "URL";
                worksheet.Cell(1, 6).Value = "Дата публикации";

                // Форматирование заголовков
                var headerRange = worksheet.Range(1, 1, 1, 6);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Заполнение данными
                for (int i = 0; i < vacancies.Count; i++)
                {
                    var vacancy = vacancies[i];
                    var row = i + 2;

                    worksheet.Cell(row, 1).Value = vacancy.Title;
                    worksheet.Cell(row, 2).Value = vacancy.Company;
                    worksheet.Cell(row, 3).Value = vacancy.Location;
                    worksheet.Cell(row, 4).Value = vacancy.Description;
                    worksheet.Cell(row, 5).Value = vacancy.Url;
                    worksheet.Cell(row, 6).Value = vacancy.PostedDate != DateTime.MinValue ? vacancy.PostedDate.ToShortDateString() : "";
                }

                // Автоширина столбцов
                worksheet.Columns().AdjustToContents();

                // Сохранение в память
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}