using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Parser.Services.VacancyParsers;

public class SuperJobVacancyParser: DefaultVacancyParser
{
    public SuperJobVacancyParser(HttpClient httpClient) : base(httpClient)
    {
    }
    
      protected override string GetFunctional(HtmlDocument htmlDocument, JsonElement parseRules)
    {
        string vacancyDescriptionXPath;
        try
        {
            vacancyDescriptionXPath = parseRules.GetProperty("DescriptionNode").GetString();
        }
        catch (Exception e)
        {
            return string.Empty;
        }

        //Нода, которая содержит общее описание о вакансии
        HtmlNode vacancyDescriptionNode = htmlDocument.DocumentNode.SelectSingleNode(vacancyDescriptionXPath);
        if (vacancyDescriptionNode == null)
        {
            return String.Empty;
        }

        return GetMainTextAfterTitle(vacancyDescriptionNode, XPathForFunctionalTitle);
    }


    protected override string GetRequirements(HtmlDocument htmlDocument, JsonElement parseRules)
    {
        string vacancyDescriptionXPath;
        try
        {
            vacancyDescriptionXPath = parseRules.GetProperty("DescriptionNode").GetString();
        }
        catch (Exception e)
        {
            return string.Empty;
        }

        //Нода, которая содержит общее описание о вакансии
        HtmlNode vacancyDescriptionNode = htmlDocument.DocumentNode.SelectSingleNode(vacancyDescriptionXPath);
        if (vacancyDescriptionNode == null)
        {
            return String.Empty;
        }

        return GetMainTextAfterTitle(vacancyDescriptionNode, XPathForRequirementTitle);
    }

    protected override string GetConditions(HtmlDocument htmlDocument, JsonElement parseRules)
    {
        string vacancyDescriptionXPath;
        try
        {
            vacancyDescriptionXPath = parseRules.GetProperty("DescriptionNode").GetString();
        }
        catch (Exception e)
        {
            return string.Empty;
        }

        //Нода, которая содержит общее описание о вакансии
        HtmlNode vacancyDescriptionNode = htmlDocument.DocumentNode.SelectSingleNode(vacancyDescriptionXPath);
        if (vacancyDescriptionNode == null)
        {
            return String.Empty;
        }

        return GetMainTextAfterTitle(vacancyDescriptionNode, XPathForConditionTitle);
    }


    private String GetMainTextAfterTitle(HtmlNode vacancyDescriptionNode, String XPathForTitle)
    {
        //Нода, которая содержит название заголовка о задачах и функционале работы
        HtmlNode titleNode = vacancyDescriptionNode.SelectSingleNode(XPathForTitle);
        StringBuilder resultText = new StringBuilder();
        if (titleNode != null)
        {
            HtmlNode nextNode = titleNode.ParentNode.NextSibling;
            while (nextNode != null && nextNode.NodeType != HtmlNodeType.Element)
            {
                nextNode = nextNode.NextSibling;
            }
            if (nextNode != null)
            {
                // Если узел является списком <ul>
                if (nextNode.Name == "ul")
                {
                    List<string> listItems = new List<string>();
                    foreach (HtmlNode liNode in nextNode.SelectNodes(".//li"))
                    {
                        resultText.Append(liNode.InnerText.Trim()).Append("\n");
                    }
                }
                // Если узел является параграфом <p> или текстовым узлом
                else if (nextNode.Name == "p" || nextNode.NodeType == HtmlNodeType.Text)
                {
                    resultText.Append(nextNode.InnerText.Trim());
                }
            }
        }

        return resultText.ToString();
    }
}