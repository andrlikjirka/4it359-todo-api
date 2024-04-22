using System.ComponentModel.DataAnnotations;

namespace TodoApp.Api.Configuration;

public class TaskCollectorOptions
{
    public bool EnableTaskCollector { get; init; }
    public int SweepInterval { get; init; } = 5000;
    [Range(1, 5, ErrorMessage = "Lowest Priority threshold must be between 1 and 5.")]
    public int MinPriorityThreshold { get; init; } = 1;
}