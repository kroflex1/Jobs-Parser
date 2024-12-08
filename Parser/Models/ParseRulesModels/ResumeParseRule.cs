using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parser.Models;

public class ResumeParseRule
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public string FullNameNode { get; set; }
    
    public string RoleNode { get; set; }
    
    public string ContactsNode { get; set; }
    
    public string CityNode { get; set; }
}