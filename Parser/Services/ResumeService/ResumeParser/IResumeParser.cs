using System.Text.Json;
using Parser.Models;

namespace Parser.Services.ResumeService.ResumeParser;

public interface IResumeParser
{
    Resume ParseResume(Uri linkToResume, JsonElement parseRule);
}