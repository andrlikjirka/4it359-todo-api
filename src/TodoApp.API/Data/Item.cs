using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApp.Api.Data;

public class Item
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Title { get; set; }

    [Range(1, 5)]
    public int Priority { get; set; }

    [Range(0, 100)]
    public int Progress { get; set; }
    
    public DateTime DueDate { get; set; }
}
