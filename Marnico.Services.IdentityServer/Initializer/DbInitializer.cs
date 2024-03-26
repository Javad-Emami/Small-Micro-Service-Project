using IdentityModel;
using Marnico.Services.IdentityServer.DbContexts;
using Marnico.Services.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Marnico.Services.IdentityServer.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public void Initialize()
        {
            if(_roleManager.FindByNameAsync(SD.Admin).Result == null)
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Customer)).GetAwaiter().GetResult();
            }
            else { return; }

            ApplicationUser adminUser = new ApplicationUser()
            {
                UserName = "admin",
                Email = "admin@admin.com",
                EmailConfirmed = true,
                PhoneNumber = "123456",
                FirstName = "john",
                LastName = "Doe"
            };
            _userManager.CreateAsync(adminUser, "Aa@123456").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(adminUser, SD.Admin).GetAwaiter().GetResult();

            var temp1 =_userManager.AddClaimsAsync(adminUser, new Claim[]
            {
                new Claim(JwtClaimTypes.Name, adminUser.FirstName + " " + adminUser.LastName),
                new Claim(JwtClaimTypes.GivenName, adminUser.FirstName),
                new Claim(JwtClaimTypes.FamilyName, adminUser.LastName),
                new Claim(JwtClaimTypes.Role, SD.Admin),
            }).Result;

            ApplicationUser customerUser = new ApplicationUser()
            {
                UserName = "customer",
                Email = "customer@customer.com",
                EmailConfirmed = true,
                PhoneNumber = "123456",
                FirstName = "john",
                LastName = "Doe"
            };
            _userManager.CreateAsync(customerUser, "Aa@123456").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(customerUser, SD.Admin).GetAwaiter().GetResult();

            var temp2 = _userManager.AddClaimsAsync(customerUser, new Claim[]
            {
                new Claim(JwtClaimTypes.Name, customerUser.FirstName + " " + customerUser.LastName),
                new Claim(JwtClaimTypes.GivenName, customerUser.FirstName),
                new Claim(JwtClaimTypes.FamilyName, customerUser.LastName),
                new Claim(JwtClaimTypes.Role, SD.Customer),
            }).Result;
        }
    }
}
