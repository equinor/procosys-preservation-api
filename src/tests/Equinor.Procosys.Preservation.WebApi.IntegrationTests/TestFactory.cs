using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Permission;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Equinor.Procosys.Preservation.WebApi.IntegrationTests.Clients;
using Equinor.Procosys.Preservation.WebApi.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests
{
    public class TestFactory : WebApplicationFactory<Startup>
    {
        private readonly string _configPath;
        public string PlantWithAccess => "PLANT1";
        public string PlantWithoutAccess => "PLANT999";
        public string ProjectWithAccess => "Project1";
        public string ProjectWithoutAccess => "Project999";

        public TestFactory()
        {
            var projectDir = Directory.GetCurrentDirectory();
            _configPath = Path.Combine(projectDir, "appsettings.json");
            AnonymousClient = CreateTestClient(null);
            LibraryAdminClient = CreateTestClient(Clients.AdminClient.Tokens);
            PlannerClient = CreateTestClient(Clients.PlannerClient.Tokens);
            PreserverClient = CreateTestClient(Clients.PreserverClient.Tokens);
            ReaderClient = CreateTestClient(Clients.ReaderClient.Tokens);
            HackerClient = CreateTestClient(Clients.HackerClient.Tokens);

            PlantApiServiceMock = new Mock<IPlantApiService>();
            PlantApiServiceMock.Setup(p => p.GetAllPlantsAsync()).Returns(Task.FromResult(
                new List<ProcosysPlant>
                {
                    new ProcosysPlant {Id = PlantWithAccess, HasAccess = true}, 
                    new ProcosysPlant {Id = PlantWithoutAccess}
                }));

            PermissionApiServiceMock = new Mock<IPermissionApiService>();
            PermissionApiServiceMock.Setup(p => p.GetAllOpenProjectsAsync(PlantWithAccess))
                .Returns(Task.FromResult<IList<ProcosysProject>>(new List<ProcosysProject>
                {
                    new ProcosysProject {Name = ProjectWithAccess, HasAccess = true},
                    new ProcosysProject {Name = ProjectWithoutAccess}
                }));
            PermissionApiServiceMock.Setup(p => p.GetPermissionsAsync(PlantWithAccess))
                .Returns(Task.FromResult<IList<string>>(new List<string> {Permissions.LIBRARY_PRESERVATION_READ}));

            PermissionApiServiceMock.Setup(p => p.GetContentRestrictionsAsync(PlantWithAccess))
                .Returns(Task.FromResult<IList<string>>(new List<string>()));

        }
        
        public HttpClient AnonymousClient { get; private set; }
        public HttpClient LibraryAdminClient { get; private set; }
        public HttpClient PlannerClient { get; private set; }
        public HttpClient PreserverClient { get; private set; }
        public HttpClient ReaderClient { get; private set; }
        public HttpClient HackerClient { get; private set; }

        public Mock<IPlantApiService> PlantApiServiceMock { get; }
        public Mock<IPermissionApiService> PermissionApiServiceMock { get; }

        public new void Dispose()
        {
            if (AnonymousClient != null)
            {
                AnonymousClient.Dispose();
                AnonymousClient = null;
            }
            if (LibraryAdminClient != null)
            {
                LibraryAdminClient.Dispose();
                LibraryAdminClient = null;
            }
            if (PlannerClient != null)
            {
                PlannerClient.Dispose();
                PlannerClient = null;
            }
            if (PreserverClient != null)
            {
                PreserverClient.Dispose();
                PreserverClient = null;
            }
            if (ReaderClient != null)
            {
                ReaderClient.Dispose();
                ReaderClient = null;
            }
            if (HackerClient != null)
            {
                HackerClient.Dispose();
                HackerClient = null;
            }

            base.Dispose();
        }
        
        protected override void ConfigureClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Add(CurrentPlantMiddleware.PlantHeader, new List<string> {PlantWithAccess});
        
            base.ConfigureClient(client);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
            => builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication()
                    .AddScheme<IntegrationTestAuthOptions, IntegrationTestAuthHandler>(
                        IntegrationTestAuthHandler.TestAuthenticationScheme, opts => { });

                services.PostConfigureAll<JwtBearerOptions>(jwtBearerOptions =>
                    jwtBearerOptions.ForwardAuthenticate = IntegrationTestAuthHandler.TestAuthenticationScheme);
                
                services.AddScoped(serviceProvider => PlantApiServiceMock.Object);
                services.AddScoped(serviceProvider => PermissionApiServiceMock.Object);
            });

        private HttpClient CreateTestClient(TestTokens tokens)
        {
            var client = WithWebHostBuilder(builder =>
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
