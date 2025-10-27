namespace Simulacro2.Dto.User;

public class UserRegisterEmpresaDto
{
    public string CompanyName { get; set; } = null!;
    public string Email { get; set; } = null!;   // formato: nombreempresa_E1@example.com
    public string Password { get; set; } = null!;
}