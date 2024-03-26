using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Marnico.Services.IdentityServer
{
    public static class SD
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
            };
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope> { new ApiScope("marnico", "Marnico Server") };

        public static IEnumerable<Client> clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId ="client",
                    ClientSecrets = {new Secret("secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes={"profile"}
                },
               new Client
                {
                    ClientId ="marnico",
                    ClientSecrets = {new Secret("secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris={ "https://localhost:44356/signin-oidc" },
                    PostLogoutRedirectUris={ "https://localhost:44356/signout-callback-oidc" },
                    AllowedScopes= new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Profile,
                        "marnico",
                    }
                },
            };
    }
}
