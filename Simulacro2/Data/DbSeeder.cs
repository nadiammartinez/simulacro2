using System.Linq;
using Simulacro2.Data;
using Simulacro2.Models;
using Simulacro2.Services;

namespace Simulacro2.Data;

public static class DbSeeder
{
    public static void SeedAdmin(AppDbContext db, PasswordService pwd, IConfiguration config)
    {
        if (db.Users.Any(u => u.Role == Role.Admin)) return;

        var email = config["Admin:Email"] ?? "admin@demo.com";
        var pass  = config["Admin:Password"] ?? "Passw0rd!";

        var admin = new User
        {
            Email = email,
            PasswordHash = pwd.Hash(pass),
            Role = Role.Admin
        };
        db.Users.Add(admin);
        db.SaveChanges();
    }
}