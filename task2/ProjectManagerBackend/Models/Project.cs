using System.ComponentModel.DataAnnotations;

namespace ProjectManagerBackend.Models;

public class Project
{
    public int Id { get; set; }

    [Required, MinLength(3), MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int OwnerId { get; set; }
    public User? Owner { get; set; }

    public List<TaskItem> Tasks { get; set; } = new();
}
