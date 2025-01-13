using System.Text.Json;
using HtmlAgilityPack;

namespace Parser.Services.ResumeService.ResumeParser.Implementations;

public class HeadHunterResumeParser : DefaultResumeParser
{
    public HeadHunterResumeParser(HttpClient httpClient) : base(httpClient)
    {
    }

    protected override List<string> GetContacts(HtmlDocument htmlDocument, JsonElement parseRules)
    {
        string? phoneContactXPath = null;
        string? emailContactXPath = null;
        string? personalContactXPath = null;
        try
        {
            phoneContactXPath = parseRules.GetProperty("PhoneContactNode").GetString();
        }
        catch (Exception e)
        {
            // ignored
        }

        try
        {
            emailContactXPath = parseRules.GetProperty("EmailContactNode").GetString();
        }
        catch (Exception e)
        {
            // ignored
        }

        try
        {
            personalContactXPath = parseRules.GetProperty("PersonalContactNode").GetString();
        }
        catch (Exception e)
        {
            // ignored
        }

        string phoneContactText = String.Empty;
        string emailContactText = String.Empty;
        string personalContactText = String.Empty;
        if (phoneContactXPath != null)
        {
            phoneContactText = htmlDocument.DocumentNode.SelectSingleNode(phoneContactXPath)?.InnerText.Trim() ?? string.Empty;
        }

        if (emailContactXPath != null)
        {
            emailContactText = htmlDocument.DocumentNode.SelectSingleNode(emailContactXPath)?.InnerText.Trim() ?? string.Empty;
        }

        if (personalContactXPath != null)
        {
            personalContactText = htmlDocument.DocumentNode.SelectSingleNode(personalContactXPath)?.InnerText.Trim() ?? string.Empty;
        }

        List<string> result = new List<string>();
        if (phoneContactText.Length != 0)
        {
            result.Add(phoneContactText);
        }

        if (emailContactText.Length != 0)
        {
            result.Add(emailContactText);
        }

        if (personalContactText.Length != 0)
        {
            result.Add(personalContactText);
        }

        return result;
    }
}