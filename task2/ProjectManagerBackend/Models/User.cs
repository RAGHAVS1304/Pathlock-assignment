using System.ComponentModel.DataAnnotations;

namespace ProjectManagerBackend.Models;

public class User
{
    public int Id { get; set; }

    [Required, MinLength(3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public List<Project> Projects { get; set; } = new();
}
