using System.Text.Json;
using HtmlAgilityPack;

namespace Parser.Services.ResumeService.ResumeParser.Implementations;

public class SuperJobResumeParser : DefaultResumeParser
{
    public SuperJobResumeParser(HttpClient httpClient) : base(httpClient)
    {
    }

    protected virtual string GetCity(HtmlDocument htmlDocument, JsonElement parseRules)
    {
        string cityXPath;
        try
        {
            cityXPath = parseRules.GetProperty("CityNode").GetString();
        }
        catch (Exception e)
        {
            return string.Empty;
        }

        HtmlNode cityNode = htmlDocument.DocumentNode.SelectSingleNode(cityXPath);
        string text = cityNode?.InnerText.Trim() ?? string.Empty;
        if (text.Length != 0)
        {
            return text.Split(',')[0];
        }

        return string.Empty;
    }
}