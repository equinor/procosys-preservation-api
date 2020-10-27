using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.WebApi.Authorizations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests
{
    internal class IntegrationTestAuthHandler : AuthenticationHandler<IntegrationTestAuthOptions>
    {
        public static string TestAuthenticationScheme = "PCS_Api_IntTest";

        private enum AuthType
        {
            Application,
            Delegated
        }

        public IntegrationTestAuthHandler(
            IOptionsMonitor<IntegrationTestAuthOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock) 
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                var claims = await GatherTestUserClaimsAsync();
                var testIdentity = new ClaimsIdentity(claims, TestAuthenticationScheme);
                var testUser = new ClaimsPrincipal(testIdentity);
                var ticket = new AuthenticationTicket(testUser, new AuthenticationProperties(), TestAuthenticationScheme);
                // Don't think there is any scenario we want to return 401, as if headers is set, the user is requested.
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex);
            }
        }

        private Task<List<Claim>> GatherTestUserClaimsAsync()
        {
            var tokenDeserialized = new
            {
                Oid = Guid.Empty, FullName = string.Empty, IsAppToken = false, Roles = new[] {string.Empty}
            };

            var authorizationHeader = Request.Headers["Authorization"];
            var tokens = authorizationHeader.ToString()?.Split(' ');
            if (tokens == null || tokens.Length <= 1)
            {
                throw new Exception("[Authorization] header missing");
            }

            var tokenPart = tokens[1];
            try
            {
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(tokenPart));
                tokenDeserialized = JsonConvert.DeserializeAnonymousType(decoded, tokenDeserialized);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Unable to extract test auth token from [Authorization] header. See inner exception for details.",
                    ex);
            }

            var oid = tokenDeserialized.Oid.ToString();
            var authType = tokenDeserialized.IsAppToken ? AuthType.Application : AuthType.Delegated;

            var claims = new List<Claim> {new Claim(ClaimsExtensions.Oid, oid)};

            switch (authType)
            {
                case AuthType.Delegated:
                    if (tokenDeserialized.Roles != null)
                    {
                        foreach (var role in tokenDeserialized.Roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }
                    }

                    if (string.IsNullOrEmpty(tokenDeserialized.FullName))
                    {
                        throw new ArgumentException("Missing name property in token");
                    }

                    AddNameClaims(claims, tokenDeserialized.FullName);
                    break;
                case AuthType.Application:
                    throw new Exception($"{authType} authentication not supported yet");
            }

            return Task.FromResult(claims);
        }

        private void AddNameClaims(List<Claim> claims, string fullName)
        {
            var tokens = fullName.Split(' ');
            if (tokens.Length > 1)
            {
                claims.Add(new Claim(ClaimTypes.Surname, tokens.Last()));
                var givenName = string.Join(" ", tokens.Take(tokens.Length - 1));
                claims.Add(new Claim(ClaimTypes.GivenName, givenName));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.GivenName, fullName));
            }
        }
    }
}
