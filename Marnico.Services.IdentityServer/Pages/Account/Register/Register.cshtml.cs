using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using IdentityModel;
using IdentityServerHost.Pages;
using Marnico.Services.IdentityServer.Models;
using Marnico.Services.IdentityServer.Pages.Account.Register;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using static IdentityServerHost.Pages.Login.ViewModel;

namespace Marnico.Services.IdentityServer.Pages.Account.Register
{
    public class RegisterModel : PageModel
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IClientStore _clientStore;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEventService _events;

        [BindProperty]
        public RegisterViewModel RegisterViewModel { get; set; }

        public RegisterModel(IIdentityServerInteractionService interaction,
                             IAuthenticationSchemeProvider schemeProvider,
                             IClientStore clientStore,
                             UserManager<ApplicationUser> userManager,
                             SignInManager<ApplicationUser> signinManager,
                             RoleManager<IdentityRole> roleManager,
                             IEventService events)
        {
            _interaction = interaction;
            _schemeProvider = schemeProvider;
            _clientStore = clientStore; 
            _userManager = userManager;
            _signinManager = signinManager;
            _roleManager = roleManager;
            _events = events;
        }
        public async Task<IActionResult> OnGet(string returnUrl)
        {
            // build a model so we know what to show on the reg page
            var vm = await BuildRegisterViewModelAsync(returnUrl);

            return RedirectToPage("./Register");
        }


        public async Task<IActionResult> OnPost(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {

                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    EmailConfirmed = true,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync(model.RoleName).GetAwaiter().GetResult())
                    {
                        var userRole = new IdentityRole
                        {
                            Name = model.RoleName,
                            NormalizedName = model.RoleName,

                        };
                        await _roleManager.CreateAsync(userRole);
                    }

                    await _userManager.AddToRoleAsync(user, model.RoleName);

                    await _userManager.AddClaimsAsync(user, new Claim[]{
                            new Claim(JwtClaimTypes.Name, model.Username),
                            new Claim(JwtClaimTypes.Email, model.Email),
                            new Claim(JwtClaimTypes.FamilyName, model.FirstName),
                            new Claim(JwtClaimTypes.GivenName, model.LastName),
                            new Claim(JwtClaimTypes.WebSite, "http://"+model.Username+".com"),
                            new Claim(JwtClaimTypes.Role,"User") });

                    var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
                    var loginresult = await _signinManager.PasswordSignInAsync(model.Username, model.Password, false, lockoutOnFailure: true);
                    if (loginresult.Succeeded)
                    {
                        var checkuser = await _userManager.FindByNameAsync(model.Username);
                        await _events.RaiseAsync(new UserLoginSuccessEvent(checkuser.UserName, checkuser.Id, checkuser.UserName, clientId: context?.Client.ClientId));

                        if (context != null)
                        {
                            if (context.IsNativeClient())
                            {
                                // The client is native, so this change in how to
                                // return the response is for better UX for the end user.
                                //return this.LoadingPage("Redirect", model.ReturnUrl);
                                return Redirect(model.ReturnUrl);
                            }

                            // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                            return Redirect(model.ReturnUrl);
                        }

                        // request for a local page
                        if (Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else if (string.IsNullOrEmpty(model.ReturnUrl))
                        {
                            return Redirect("~/");
                        }
                        else
                        {
                            // user might have clicked on a malicious link - should be logged
                            throw new Exception("invalid return URL");
                        }
                    }

                }
            }

            // If we got this far, something failed, redisplay form
            return Redirect("./Register");
        }

        private async Task<RegisterViewModel> BuildRegisterViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            List<string> roles = new List<string>();
            roles.Add("Admin");
            roles.Add("Customer");
            ViewData["message"] = roles;
            //ViewBag.message = roles;
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                var vm = new RegisterViewModel
                {
                    EnableLocalLogin = local,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                };

                if (!local)
                {
                    vm.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
                }

                return vm;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null)
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName ?? x.Name,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;
            if (context?.Client.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            return new RegisterViewModel
            {
                AllowRememberLogin = RegisterOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && RegisterOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }
    }
}
