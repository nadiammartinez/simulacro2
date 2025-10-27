using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simulacro2.Data;
using Simulacro2.Dto.User;
using Simulacro2.Services;

namespace Simulacro2.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountController(AppDbContext context, PasswordService pwd) : ControllerBase
{
    [HttpPut("change-password")]
    public async Task<ActionResult> ChangePassword(ChangePasswordDto dto)
    {
        if (dto.NewPassword != dto.RepeatNewPassword) return BadRequest("Las contraseñas no coinciden");

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return NotFound();

        if (!pwd.Verify(dto.CurrentPassword, user.PasswordHash)) return Unauthorized();

        user.PasswordHash = pwd.Hash(dto.NewPassword);
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteAccount([FromQuery] string password)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return NotFound();

        if (!pwd.Verify(password, user.PasswordHash)) return Unauthorized();

        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return NoContent();
    }
}