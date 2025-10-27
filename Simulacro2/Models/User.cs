using System.ComponentModel.DataAnnotations;

namespace Simulacro2.Models;

public enum Role { Admin = 0, Empresa = 1, Cliente = 2 }

public class User
{
    [Key] public int Id { get; set; }

    [Required, EmailAddress] public string Email { get; set; } = null!;
    [Required, MinLength(6)] public string PasswordHash { get; set; } = null!;
    [Required] public Role Role { get; set; }

    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? ProfileImageUrl { get; set; }

    // Solo para cuentas Empresa
    public string? CompanyName { get; set; }
}