#nullable enable
using System.ComponentModel.DataAnnotations;

namespace TodoApp.Api.Messages;

public class QueryRequest
{
    [MaxLength(100)]
    public string? Name { get; set; }
    
    [Range(0, 100)]
    public int? ProgressFrom { get; set; }

    [Range(0, 100)]
    public int? ProgressTo { get; set; }
    
    public DateTime? DueDateFrom { get; set; }
    
    public DateTime? DueDateTo { get; set; }

    [Required]
    [Range(1, Int32.MaxValue)]
    public int Limit { get; set; }
    
}