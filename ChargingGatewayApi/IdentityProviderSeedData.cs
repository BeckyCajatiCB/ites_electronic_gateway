using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Models;

namespace ChargingGatewayApi
{
    public class IdentityProviderSeedData
    {
        public static IEnumerable<ApiResource> GetApiResourceses()
        {
            return new List<ApiResource>
            {
                new ApiResource
                {
                    Name = "sample_api",
                    ApiSecrets = {new Secret("api_secret".Sha256())},
                    Scopes = {new Scope("sample_api")}
                }
            };
        }


        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "sample_client",
                    AllowedGrantTypes = {GrantType.ClientCredentials},
                    ClientSecrets = {new Secret("client_secret".Sha256())},
                    AllowedScopes = {"sample_api"},
                    AccessTokenType = AccessTokenType.Reference,
                    Claims = {new Claim(JwtClaimTypes.Role, "sample_api.admin")}
                }
            };
        }
    }
}