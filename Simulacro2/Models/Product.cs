using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simulacro2.Models;

public class Product
{
    [Key] public int Id { get; set; }

    [Required, MaxLength(120)] public string Name { get; set; } = null!;
    [MaxLength(500)] public string? Description { get; set; }
    [Range(0, double.MaxValue)] public decimal Price { get; set; }
    public string? ImageUrl { get; set; }

    [ForeignKey(nameof(CompanyUser))] public int CompanyUserId { get; set; }
    public User CompanyUser { get; set; } = null!;
}