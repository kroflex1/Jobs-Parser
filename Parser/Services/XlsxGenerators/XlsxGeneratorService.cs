using ClosedXML.Excel;
using Parser.Models;

namespace Parser.Services.XlsxGenerators
{
    public class XlsxGeneratorService
    {
        public byte[] GenerateXlsx(List<Vacancy> vacancies, String sheetName = "Вакансии")
        {
            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add(sheetName);
                prepareTitle(worksheet);
                fillData(worksheet, vacancies);
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        private void prepareTitle(IXLWorksheet worksheet)
        {
            IXLRow titleRow = worksheet.Row(1);
            int cellNumber = 1;
            titleRow.Cell(cellNumber++).Value = "Компания";
            titleRow.Cell(cellNumber++).Value = "Название вакансии";
            titleRow.Cell(cellNumber++).Value = "Роль";
            titleRow.Cell(cellNumber++).Value = "Грейд вакансии";
            titleRow.Cell(cellNumber++).Value = "Город и удалёнка";
            titleRow.Cell(cellNumber++).Value = "Ссылка";
            titleRow.Cell(cellNumber++).Value = "Задача и функционал";
            titleRow.Cell(cellNumber++).Value = "Требования";
            titleRow.Cell(cellNumber++).Value = "Ключевые навыки";
            titleRow.Cell(cellNumber++).Value = "Условия";
            titleRow.Cell(cellNumber++).Value = "ЗП от";
            titleRow.Cell(cellNumber++).Value = "ЗП до";
            titleRow.Cell(cellNumber++).Value = "Результирующий уровень ЗП";

            IXLRange headerRange = worksheet.Range(1, 1, 1, --cellNumber);
            headerRange.SetAutoFilter();
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        private void fillData(IXLWorksheet worksheet, List<Vacancy> vacancies)
        {
            for (int i = 0; i < vacancies.Count; i++)
            {
                Vacancy vacancy = vacancies[i];
                IXLRow row = worksheet.Row(i + 2);
                int cellNumber = 1;
                row.Cell(cellNumber++).Value = vacancy.CompanyName; //Компания
                row.Cell(cellNumber++).Value = vacancy.Name; //Название вакансии
                row.Cell(cellNumber++).Value = vacancy.Role; //Роль
                row.Cell(cellNumber++).Value = vacancy.Grade; //Грейд вакансии
                row.Cell(cellNumber++).Value = vacancy.City; //Город и удалёнка
                row.Cell(cellNumber++).Value = vacancy.LinkToSource.ToString(); //Ссылка
                row.Cell(cellNumber++).Value = vacancy.Functional; //Задача и функционал
                row.Cell(cellNumber++).Value = vacancy.Requirements; //Требования
                row.Cell(cellNumber++).Value = vacancy.KeySkills; //Ключевые навыки
                row.Cell(cellNumber++).Value = vacancy.Conditions; //Условия
                row.Cell(cellNumber++).Value = vacancy.SalaryFrom; //ЗП от
                row.Cell(cellNumber++).Value = vacancy.SalaryTo; //ЗП до
                row.Cell(cellNumber).Value = (vacancy.SalaryFrom + vacancy.SalaryTo) / 2.0; //Результирующий уровень ЗП
            }
        }
    }
}