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
}