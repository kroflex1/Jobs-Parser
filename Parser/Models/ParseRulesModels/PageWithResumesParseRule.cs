using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parser.Models;

public class PageWithResumesParseRule
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    [Required]
    public string UrlWithResumes { get; set; }
    
    public string ParamNameForResumeTitle { get; set; }
    
    public string ResumeUrlNode { get; set; }
    
    public string NextPageNode { get; set; }
}