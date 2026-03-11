using System.ComponentModel.DataAnnotations;
using Ventrix.Domain.Enums;

public class User
{
    [Key]
    public string? UserId { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? Suffix { get; set; }
    public UserRole Role { get; set; }
    public string? Password { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int Strikes { get; set; } = 0;

    public bool IsLockedOut => Strikes >= 3;

    public string FullName => $"{FirstName} {LastName} {Suffix}".Trim();
}