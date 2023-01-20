using BulkyBook.DataAccess.Context;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BulkyBook.DataAccess.DbInitializer;

public class DbInitializer : IDbInitializer
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public DbInitializer(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _configuration = configuration;
    }

    public async Task InitializeAsync()
    {
        // Apply migrations if they are not applied
        var migrations = await _context.Database.GetPendingMigrationsAsync();
        if (migrations.Any())
        {
            await _context.Database.MigrateAsync();
        }

        // Create roles if they are not created
        if (!await _roleManager.RoleExistsAsync(SD.ROLE_USER_ADMIN))
        {
            await _roleManager.CreateAsync(new IdentityRole(SD.ROLE_USER_ADMIN));
            await _roleManager.CreateAsync(new IdentityRole(SD.ROLE_USER_COMPANY));
            await _roleManager.CreateAsync(new IdentityRole(SD.ROLE_USER_EMPLOYEE));
            await _roleManager.CreateAsync(new IdentityRole(SD.ROLE_USER_INDIVIDUAL));

            string userAdmEmail = "admin@gmail.com";

            // Create admin user
            await _userManager.CreateAsync(new ApplicationUser
            {
                UserName = userAdmEmail,
                Email = userAdmEmail,
                Name = "Administrator",
                PhoneNumber = "99999999",
                StreetAddress = "123 Ave",
                State = "IL",
                PostalCode = "43421",
                City = "Chicago"
            },
            _configuration["DbInitializer:MasterPassword"]);

            ApplicationUser user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == userAdmEmail);
            await _userManager.AddToRoleAsync(user, SD.ROLE_USER_ADMIN);
        }
    }
}
