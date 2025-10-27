using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simulacro2.Data;
using Simulacro2.Dto.Auth;
using Simulacro2.Dto.User;
using Simulacro2.Models;
using Simulacro2.Services;

namespace Simulacro2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AppDbContext context, TokenService tokenService, PasswordService pwd) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<TokenDto>> Login(UserLoginDto dto)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null) return Unauthorized();

        if (!pwd.Verify(dto.Password, user.PasswordHash)) return Unauthorized();

        var jwt = tokenService.GenerateToken(user);
        return new TokenDto { Token = jwt };
    }

    [HttpPost("registerCliente")]
    public async Task<ActionResult> RegisterCliente(UserRegisterClienteDto dto)
    {
        var exists = await context.Users.AnyAsync(u => u.Email == dto.Email);
        if (exists) return BadRequest("Email ya registrado");

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = pwd.Hash(dto.Password),
            Role = Role.Cliente,
            Name = dto.Name,
            LastName = dto.LastName,
            UserName = dto.UserName,
            Address = dto.Address,
            Phone = dto.Phone
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return StatusCode(201);
    }

    [HttpPost("registerEmpresa")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> RegisterEmpresa(UserRegisterEmpresaDto dto)
    {
        var exists = await context.Users.AnyAsync(u => u.Email == dto.Email);
        if (exists) return BadRequest("Email ya registrado");

        var empresa = new User
        {
            Email = dto.Email,
            PasswordHash = pwd.Hash(dto.Password),
            Role = Role.Empresa,
            CompanyName = dto.CompanyName
        };
        context.Users.Add(empresa);
        await context.SaveChangesAsync();
        return StatusCode(201);
    }

    [HttpPost("registerAdmin")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> RegisterAdmin(UserRegisterAdminDto dto)
    {
        var exists = await context.Users.AnyAsync(u => u.Email == dto.Email);
        if (exists) return BadRequest("Email ya registrado");

        var admin = new User
        {
            Email = dto.Email,
            PasswordHash = pwd.Hash(dto.Password),
            Role = Role.Admin
        };
        context.Users.Add(admin);
        await context.SaveChangesAsync();
        return StatusCode(201);
    }
}
