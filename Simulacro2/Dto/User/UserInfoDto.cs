using Simulacro2.Models;

namespace Simulacro2.Dto.User;

public class UserInfoDto
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public Role Role { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? CompanyName { get; set; }
}