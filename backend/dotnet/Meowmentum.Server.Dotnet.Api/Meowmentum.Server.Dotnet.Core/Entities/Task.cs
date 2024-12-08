using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meowmentum.Server.Dotnet.Core.Entities;

public class Task
{
    [Key]
    public long Id { get; set; }
    [Required]
    [MaxLength(200)]
    public required string Title { get; set; }
    public string? Description { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    public DateTime? Deadline { get; set; }
    public TaskStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; } 
    [Required]
    [ForeignKey("User")]
    public long UserId { get; set; }
    public virtual ICollection<TimeInterval>? TimeIntervals { get; set; }
    public virtual ICollection<TaskTag>? TaskTags { get; set; }
}

public enum TaskStatus
{
    Pending,
    InProgress,
    Completed
}

public enum TaskPriority
{
    High,
    Medium,
    Low
}
