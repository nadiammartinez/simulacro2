using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simulacro2.Data;
using Simulacro2.Dto.User;

namespace Simulacro2.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<UserInfoDto>> GetProfile()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var user = await context.Users
            .Where(u => u.Id == userId)
            .Select(u => new UserInfoDto
            {
                Id = u.Id,
                Email = u.Email,
                Role = u.Role,
                Name = u.Name,
                LastName = u.LastName,
                UserName = u.UserName,
                Address = u.Address,
                Phone = u.Phone,
                ProfileImageUrl = u.ProfileImageUrl,
                CompanyName = u.CompanyName
            })
            .FirstOrDefaultAsync();

        if (user == null) return NotFound();
        return user;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateProfile(UserUpdateDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        user.Name = dto.Name ?? user.Name;
        user.LastName = dto.LastName ?? user.LastName;
        user.UserName = dto.UserName ?? user.UserName;
        user.Address = dto.Address ?? user.Address;
        user.Phone = dto.Phone ?? user.Phone;
        user.ProfileImageUrl = dto.ProfileImageUrl ?? user.ProfileImageUrl;

        await context.SaveChangesAsync();
        return NoContent();
    }
}