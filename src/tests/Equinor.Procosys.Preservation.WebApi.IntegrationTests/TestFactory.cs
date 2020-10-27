using System.IO;
using System.Net.Http;
using Equinor.Procosys.Preservation.WebApi.IntegrationTests.Clients;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests
{
    public class TestFactory : WebApplicationFactory<Startup>
    {
        private readonly string _configPath;

        public TestFactory()
        {
            var projectDir = Directory.GetCurrentDirectory();
            _configPath = Path.Combine(projectDir, "appsettings.json");
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
            => builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication()
                    .AddScheme<IntegrationTestAuthOptions, IntegrationTestAuthHandler>(
                        IntegrationTestAuthHandler.TestAuthenticationScheme, opts => { });

                services.PostConfigureAll<JwtBearerOptions>(jwtBearerOptions =>
                    jwtBearerOptions.ForwardAuthenticate = IntegrationTestAuthHandler.TestAuthenticationScheme);
            });

        public HttpClient CreateTestClient(TestTokens tokens)
        {
            var client = this.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment(Startup.IntegrationTestEnvironment);
                builder.ConfigureAppConfiguration((context, conf) => conf.AddJsonFile(_configPath));

            }).CreateClient();

            if (tokens != null)
            {
                client.DefaultRequestHeaders.Add("Authorization", BearerTokenUtilility.WrapAuthToken(tokens));
            }
            return client;
        }
    }
}
