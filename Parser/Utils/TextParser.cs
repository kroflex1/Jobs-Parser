using System.Text;
using System.Text.RegularExpressions;

namespace Parser.Utils;

public  static class TextParser
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
            firstNumber = int.Parse(matches[0].Value.Replace(" ", ""));
        }
        if (matches.Count > 1)
        {
            secondNumber = int.Parse(matches[1].Value.Replace(" ", ""));
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
}