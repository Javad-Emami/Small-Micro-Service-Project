﻿using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Marnico.Services.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Marnico.Services.IdentityServer.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ProfileService(IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
                              UserManager<ApplicationUser> userManager,
                              RoleManager<IdentityRole> roleManager)           
        {
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            string sub = context.Subject.GetType().Name;
            ApplicationUser user = await _userManager.FindByNameAsync(sub);
            ClaimsPrincipal userClaims = await _userClaimsPrincipalFactory.CreateAsync(user);

            List<Claim> claims = userClaims.Claims.ToList();
            claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();

            if (_userManager.SupportsUserRole) 
            { 
                IList<string> roles = await _userManager.GetRolesAsync(user);
                foreach(var roleName in roles)
                {
                    claims.Add(new Claim(JwtClaimTypes.Role, roleName));
                    claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));
                    claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));

                    if (_roleManager.SupportsRoleClaims)
                    {
                        IdentityRole role = await _roleManager.FindByNameAsync(roleName);
                        if(role != null)
                        {
                            claims.AddRange(await _roleManager.GetClaimsAsync(role));
                        }
                    }
                }
            }
            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            string sub = context.Subject.GetType().Name;
            ApplicationUser user = await _userManager.FindByNameAsync(sub);
            context.IsActive = user != null;
        }
    }
}
