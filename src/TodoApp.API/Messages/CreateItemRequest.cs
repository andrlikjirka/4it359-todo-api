using System.ComponentModel.DataAnnotations;

namespace TodoApp.Api.Messages;

public class CreateItemRequest
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; }
    
    [Range(1, 5)]
    public int Priority { get; set; } = 5;

    [Range(0, 100)]
    public int Progress { get; set; }
    
    public DateTime DueDate { get; set; }
}