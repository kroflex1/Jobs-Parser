using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Parser.Utils;

public static class TextParser
{
    /// <summary>
    /// Извлекает два числа из строки. Если первое или второе число отсутствует, то возвращает 0 в качестве их значения.
    /// </summary>
    /// <param name="input">Текст, содержащий диапазон чисел</param>
    /// <returns>Кортеж, содержащий два числа. Значение будет равно 0, если его нет тексте нет.</returns>
    public static Tuple<int, int> ExtractNumbers(string input)
    {
        // Регулярное выражение для чисел, допускающих пробелы внутри (например, "70 000")
        Regex regex = new Regex(@"\d{1,3}(?:\s\d{3})*");
        MatchCollection matches = regex.Matches(input);

        int firstNumber = 0;
        int secondNumber = 0;

        if (matches.Count == 0)
        {
            return Tuple.Create(0, 0);
        }

        if (matches.Count > 0)
        {
            firstNumber = int.Parse(Regex.Replace(matches[0].Value, @"\s+", ""));
        }

        if (matches.Count > 1)
        {
            secondNumber = int.Parse(Regex.Replace(matches[1].Value, @"\s+", ""));
        }

        return Tuple.Create(firstNumber, secondNumber);
    }

    /// <summary>
    /// Создаёт XPath, с помощью которого можно найти ноду, содержащее одно из ключевых слов.
    /// </summary>
    /// <param name="keywords">Список слов, которое может содержать нода</param>
    /// <returns>XPath, с помощью которого можно найти определенную ноду.</returns>
    public static string CreateXPathForNodeWithText(List<string> keywords)
    {
        if (keywords.Count == 0)
        {
            return String.Empty;
        }

        StringBuilder xpathBuilder = new StringBuilder(".//*[");

        for (int i = 0; i < keywords.Count; i++)
        {
            // Добавляем часть XPath для текущего ключевого слова
            xpathBuilder.Append($"contains(text(), '{keywords[i]}')");

            // Добавляем "or" между условиями, кроме последнего
            if (i < keywords.Count - 1)
            {
                xpathBuilder.Append(" or ");
            }
        }

        xpathBuilder.Append("]");
        return xpathBuilder.ToString();
    }


    /// <summary>
    /// Парсит дату.
    /// </summary>
    /// <param name="dateText">Строкове значение даты</param>
    /// <returns>Дату</returns>
    public static DateTime? ParseDate(string dateText)
    {
        // Локаль для русского языка
        CultureInfo culture = new CultureInfo("ru-RU");
        string[] formats = { "d MMMM yyyy", "d MMMM", "HH:mm" }; // Форматы для даты с годом; без года; только время

        DateTime result;

        if (DateTime.TryParseExact(dateText, formats, culture, DateTimeStyles.None, out result))
        {
            // Если указан только месяц и день, добавляем текущий год
            if (!Regex.IsMatch(dateText, @"\b\d{4}\b"))
            {
                result = result.AddYears(DateTime.Now.Year - result.Year);
            }

            // Если указано только время, добавляем текущий день, месяц и год
            if (dateText.Contains(":"))
            {
                result = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, result.Hour, result.Minute, 0);
            }
        }
        else
        {
            return null;
        }

        return result;
    }


    public static JsonElement ParseStringToJsonElement(string jsonString)
    {
        using JsonDocument jsonDocument = JsonDocument.Parse(jsonString);
        return jsonDocument.RootElement.Clone(); // Клонируем JsonElement для возврата
    }

    public static string GetTextWithoutChildren(HtmlNode node)
    {
        if (node == null)
            return String.Empty;

        // Получаем полный текст узла
        string fullText = node.InnerText;

        // Вычитаем текст всех дочерних узлов
        foreach (var child in node.ChildNodes)
        {
            if (child.NodeType == HtmlNodeType.Text)
                continue; // Пропускаем текстовые узлы самой ноды

            string childText = child.InnerText;
            if (!string.IsNullOrEmpty(childText))
            {
                fullText = fullText.Replace(childText, string.Empty);
            }
        }

        return fullText.Trim();
    }
}