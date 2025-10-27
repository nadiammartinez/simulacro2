namespace Simulacro2.Dto.User;

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
    public string RepeatNewPassword { get; set; } = null!;
}