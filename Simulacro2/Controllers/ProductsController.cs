using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simulacro2.Data;
using Simulacro2.Dto.Products;
using Simulacro2.Models;

namespace Simulacro2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(AppDbContext context) : ControllerBase
{
    // 1) Lista de empresas para la pantalla principal
    [HttpGet("companies")]
    public async Task<ActionResult<IEnumerable<object>>> GetCompanies()
    {
        var companies = await context.Users
            .Where(u => u.Role == Role.Empresa)
            .Select(u => new
            {
                companyUserId = u.Id,
                companyName = u.CompanyName ?? u.UserName ?? u.Email,
                imageUrl = u.ProfileImageUrl
            })
            .ToListAsync();

        return companies;
    }

    // 2) Productos de una empresa específica
    [HttpGet("company/{companyUserId:int}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetCompanyProducts(int companyUserId)
    {
        var exists = await context.Users.AnyAsync(u => u.Id == companyUserId && u.Role == Role.Empresa);
        if (!exists) return NotFound();

        var items = await context.Products
            .Where(p => p.CompanyUserId == companyUserId)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                CompanyUserId = p.CompanyUserId,
                CompanyName = p.CompanyUser.CompanyName ?? p.CompanyUser.Email
            })
            .ToListAsync();

        return items;
    }

    // 3) Crear producto (solo Empresa)
    [HttpPost]
    [Authorize(Roles = "Empresa")]
    public async Task<ActionResult<ProductDto>> Create(ProductCreateUpdateDto dto)
    {
        var companyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var entity = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            ImageUrl = dto.ImageUrl,
            CompanyUserId = companyId
        };

        context.Products.Add(entity);
        await context.SaveChangesAsync();

        var created = await context.Products
            .Where(p => p.Id == entity.Id)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                CompanyUserId = p.CompanyUserId,
                CompanyName = p.CompanyUser.CompanyName ?? p.CompanyUser.Email
            })
            .FirstAsync();

        return CreatedAtAction(nameof(GetCompanyProducts), new { companyUserId = companyId }, created);
    }

    // 4) Editar producto (solo de la propia empresa)
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Empresa")]
    public async Task<ActionResult> Update(int id, ProductCreateUpdateDto dto)
    {
        var companyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var entity = await context.Products.FirstOrDefaultAsync(p => p.Id == id && p.CompanyUserId == companyId);
        if (entity == null) return NotFound();

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Price = dto.Price;
        entity.ImageUrl = dto.ImageUrl;

        await context.SaveChangesAsync();
        return NoContent();
    }

    // 5) Eliminar producto (solo de la propia empresa)
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Empresa")]
    public async Task<ActionResult> Delete(int id)
    {
        var companyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var entity = await context.Products.FirstOrDefaultAsync(p => p.Id == id && p.CompanyUserId == companyId);
        if (entity == null) return NotFound();

        context.Products.Remove(entity);
        await context.SaveChangesAsync();
        return NoContent();
    }
}
