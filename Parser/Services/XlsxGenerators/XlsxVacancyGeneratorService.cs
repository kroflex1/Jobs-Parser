using System.Collections;
using System.Text;
using ClosedXML.Excel;
using Parser.Models;

namespace Parser.Services.XlsxGenerators
{
    public class XlsxVacancyGeneratorService
    {
        public byte[] GenerateXlsx(List<Vacancy> vacancies, String sheetName = "Вакансии")
        {
            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet vacanciesSheet = workbook.Worksheets.Add(sheetName);
                PrepareTitleForVacanciesSheet(vacanciesSheet);
                FillDataForVacanciesSheet(vacanciesSheet, vacancies);
                ApplyStyleForVacanciesSheet(vacanciesSheet);

                IXLWorksheet percentileSheet = workbook.Worksheets.Add("Перцентили");
                PrepareTitleForPercentileSheet(percentileSheet);
                FillDataForPercentileSheet(percentileSheet, vacancies);
                ApplyStyleForPercentileSheet(percentileSheet);

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        private void PrepareTitleForVacanciesSheet(IXLWorksheet worksheet)
        {
            IXLRow titleRow = worksheet.Row(1);
            int cellNumber = 1;
            titleRow.Cell(cellNumber++).Value = "Компания";
            titleRow.Cell(cellNumber++).Value = "Название вакансии";
            titleRow.Cell(cellNumber++).Value = "Город";
            titleRow.Cell(cellNumber++).Value = "Формат работы";
            titleRow.Cell(cellNumber++).Value = "Ссылка";
            titleRow.Cell(cellNumber++).Value = "Задача и функционал";
            titleRow.Cell(cellNumber++).Value = "Требования";
            titleRow.Cell(cellNumber++).Value = "Условия";
            titleRow.Cell(cellNumber++).Value = "Ключевые навыки";
            titleRow.Cell(cellNumber++).Value = "ЗП от";
            titleRow.Cell(cellNumber++).Value = "ЗП до";
            titleRow.Cell(cellNumber++).Value = "Результирующий уровень ЗП";

            IXLRange headerRange = worksheet.Range(1, 1, 1, --cellNumber);
            headerRange.SetAutoFilter();
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
        }

        private void FillDataForVacanciesSheet(IXLWorksheet worksheet, List<Vacancy> vacancies)
        {
            for (int i = 0; i < vacancies.Count; i++)
            {
                Vacancy vacancy = vacancies[i];
                IXLRow row = worksheet.Row(i + 2);
                int cellNumber = 1;
                row.Cell(cellNumber++).Value = vacancy.CompanyName; //Компания
                row.Cell(cellNumber++).Value = vacancy.Name; //Название вакансии
                row.Cell(cellNumber++).Value =
                    vacancy.City + (vacancy.WorkFormat != null && vacancy.WorkFormat.Contains(WorkFormat.REMOTE) ? " или удалённо" : ""); //Город
                row.Cell(cellNumber++).Value = GetValueForWorkFormat(vacancy); //Формат работы
                row.Cell(cellNumber).Value = vacancy.LinkToSource.ToString(); //Ссылка
                row.Cell(cellNumber++).SetHyperlink(new XLHyperlink(vacancy.LinkToSource.ToString()));
                row.Cell(cellNumber++).Value = vacancy.Functional; //Задача и функционал
                row.Cell(cellNumber++).Value = vacancy.Requirements; //Требования
                row.Cell(cellNumber++).Value = vacancy.Conditions; //Условия
                row.Cell(cellNumber++).Value = vacancy.KeySkills; //Ключевые навыки
                row.Cell(cellNumber++).Value = vacancy.SalaryFrom; //ЗП от
                row.Cell(cellNumber++).Value = vacancy.SalaryTo == null || vacancy.SalaryTo == 0 ? "" : vacancy.SalaryTo; //ЗП до
                row.Cell(cellNumber).Value = vacancy.getAverageSalaryValue(); //Результирующий уровень ЗП
            }
        }

        private void ApplyStyleForVacanciesSheet(IXLWorksheet worksheet)
        {
            // Включаем перенос текста для всех ячеек
            worksheet.Style.Alignment.WrapText = true;
            // Устанавливаем фиксированную ширину для всех строк
            worksheet.Columns().Width = 20;
            // Устанавливаем фиксированную высоту для всех строк
            worksheet.Rows().Height = 30;
            // Выравниваем значения
            // Применяем выравнивание ко всем ячейкам
            worksheet.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
        }


        private void PrepareTitleForPercentileSheet(IXLWorksheet worksheet)
        {
            IXLRow titleRow = worksheet.Row(1);
            int cellNumber = 1;
            titleRow.Cell(cellNumber++).Value = "Перцентиль";
            titleRow.Cell(cellNumber++).Value = "Значение";

            IXLRange headerRange = worksheet.Range(1, 1, 1, --cellNumber);
            headerRange.SetAutoFilter();
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
        }

        private void FillDataForPercentileSheet(IXLWorksheet worksheet, List<Vacancy> vacancies)
        {
            List<int> sortedAverageSalary = vacancies
                .Where(vacancy => vacancy.getAverageSalaryValue() != 0)
                .Select(vacancy => Convert.ToInt32(vacancy.getAverageSalaryValue()))
                .ToList();
            sortedAverageSalary.Sort();

            List<int> percentiles = new List<int> { 90, 75, 50, 25, 10 };
            for (int i = 0; i < percentiles.Count; i++)
            {
                IXLRow row = worksheet.Row(i + 2);
                int cellNumber = 1;
                int percentile = GetPercentileFromSortedValues(percentiles[i], sortedAverageSalary);
                row.Cell(cellNumber++).Value = percentiles[i] + "%"; //Перцентиль
                row.Cell(cellNumber).Value = percentile == -1 ? "" : percentile; //Значение перцентиля
            }
        }

        private void ApplyStyleForPercentileSheet(IXLWorksheet worksheet)
        {
            // Устанавливаем фиксированную ширину для всех строк
            worksheet.Columns().Width = 20;
            // Выравниваем значения
            // Применяем выравнивание ко всем ячейкам
            worksheet.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            worksheet.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }


        private int GetPercentileFromSortedValues(int percentile, List<int> sortedValues)
        {
            if (sortedValues.Count == 0)
            {
                return -1;
            }

            return sortedValues[percentile * sortedValues.Count / 100];
        }

        private String GetValueForWorkFormat(Vacancy vacancy)
        {
            List<string> result = new List<string>();
            if (vacancy.WorkFormat == null || vacancy.WorkFormat.Count == 0)
            {
                return "На месте";
            }
            foreach (WorkFormat workFormat in vacancy.WorkFormat)
            {
                if (workFormat.Equals(WorkFormat.INTERNAL))
                {
                    result.Add("На месте");
                }
                else if (workFormat.Equals(WorkFormat.REMOTE))
                {
                    result.Add("Удалённо");
                }
                else if (workFormat.Equals(WorkFormat.HYBRID))
                {
                    result.Add("Гибрид");
                }
            }
            return String.Join("/", result);
        }
    }
}