using ClosedXML.Excel;
using Parser.Models;

namespace Parser.Services.XlsxGenerators;

public class XlsxResumeGeneratorService
{
    public byte[] GenerateXlsx(List<Resume> resumes, String sheetName = "Резюме")
    {
        using (var workbook = new XLWorkbook())
        {
            IXLWorksheet vacanciesSheet = workbook.Worksheets.Add(sheetName);
            PrepareTitleForResumesSheet(vacanciesSheet);
            FillDataForResumesSheet(vacanciesSheet, resumes);
            ApplyStyleForResumesSheet(vacanciesSheet);

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }
    }

    private void PrepareTitleForResumesSheet(IXLWorksheet worksheet)
    {
        IXLRow titleRow = worksheet.Row(1);
        int cellNumber = 1;
        titleRow.Cell(cellNumber++).Value = "Должность";
        titleRow.Cell(cellNumber++).Value = "Контакты";
        titleRow.Cell(cellNumber++).Value = "Ссылка";
        titleRow.Cell(cellNumber++).Value = "Город проживания";
        titleRow.Cell(cellNumber++).Value = "Ожидаемая ЗП";

        IXLRange headerRange = worksheet.Range(1, 1, 1, --cellNumber);
        headerRange.SetAutoFilter();
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
    }

    private void FillDataForResumesSheet(IXLWorksheet worksheet, List<Resume> resumes)
    {
        for (int i = 0; i < resumes.Count; i++)
        {
            Resume resume = resumes[i];
            IXLRow row = worksheet.Row(i + 2);
            int cellNumber = 1;
            row.Cell(cellNumber++).Value = resume.Role; //Должность
            row.Cell(cellNumber++).Value = string.Join("\n", resume.Contacts); //Контакты
            row.Cell(cellNumber).Value = resume.LinkToSource.ToString(); //Ссылка
            row.Cell(cellNumber++).SetHyperlink(new XLHyperlink(resume.LinkToSource.ToString()));
            row.Cell(cellNumber++).Value = resume.City; //Город проживания
            if (resume.getAverageSalaryValue() != 0)
            {
                row.Cell(cellNumber++).Value = resume.getAverageSalaryValue(); //Ожидаемая ЗП
            }
        }
    }

    private void ApplyStyleForResumesSheet(IXLWorksheet worksheet)
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
}