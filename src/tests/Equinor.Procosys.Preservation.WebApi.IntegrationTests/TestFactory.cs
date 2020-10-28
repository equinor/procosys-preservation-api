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
        private HttpClient _anonymousClient;
        private HttpClient _libraryAdminClient;
        private HttpClient _plannerClient;
        private HttpClient _preserverClient;
        private HttpClient _hackerClient;

        public string PlantWithAccess => "PLANT1";
        public string PlantWithoutAccess => "PLANT999";
        public string ProjectWithAccess => "Project1";
        public string ProjectWithoutAccess => "Project999";

        public TestFactory()
        {
            var projectDir = Directory.GetCurrentDirectory();
            _configPath = Path.Combine(projectDir, "appsettings.json");

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

            PermissionApiServiceMock.Setup(p => p.GetContentRestrictionsAsync(PlantWithAccess))
                .Returns(Task.FromResult<IList<string>>(new List<string>()));

            _anonymousClient = CreateTestClient(null);
            _libraryAdminClient = CreateTestClient(LibraryAdminClient.Tokens);
            _plannerClient = CreateTestClient(PlannerClient.Tokens);
            _preserverClient = CreateTestClient(PreserverClient.Tokens);
            _hackerClient = CreateTestClient(HackerClient.Tokens);
        }

        public HttpClient GetAnonymousClient()
        {
            PermissionApiServiceMock.Setup(p => p.GetPermissionsAsync(PlantWithAccess))
                .Returns(Task.FromResult<IList<string>>(null));
            return _anonymousClient;
        }

        public HttpClient GetLibraryAdminClient()
        {
            PermissionApiServiceMock.Setup(p => p.GetPermissionsAsync(PlantWithAccess))
                .Returns(Task.FromResult(LibraryAdminClient.ProCoSysPermissions));
            return _libraryAdminClient;
        }

        public HttpClient GetPlannerClient()
        {
            PermissionApiServiceMock.Setup(p => p.GetPermissionsAsync(PlantWithAccess))
                .Returns(Task.FromResult(PlannerClient.ProCoSysPermissions));
            return _plannerClient;
        }

        public HttpClient GetPreserverClient()
        {
            PermissionApiServiceMock.Setup(p => p.GetPermissionsAsync(PlantWithAccess))
                .Returns(Task.FromResult(PreserverClient.ProCoSysPermissions));
            return _preserverClient;
        }

        public HttpClient GetHackerClient()
        {
            PermissionApiServiceMock.Setup(p => p.GetPermissionsAsync(PlantWithAccess))
                .Returns(Task.FromResult<IList<string>>(new List<string>()));
            return _hackerClient;
        }

        public Mock<IPlantApiService> PlantApiServiceMock { get; }
        public Mock<IPermissionApiService> PermissionApiServiceMock { get; }

        public new void Dispose()
        {
            if (_anonymousClient != null)
            {
                _anonymousClient.Dispose();
                _anonymousClient = null;
            }
            if (_libraryAdminClient != null)
            {
                _libraryAdminClient.Dispose();
                _libraryAdminClient = null;
            }
            if (_plannerClient != null)
            {
                _plannerClient.Dispose();
                _plannerClient = null;
            }
            if (_preserverClient != null)
            {
                _preserverClient.Dispose();
                _preserverClient = null;
            }
            if (_hackerClient != null)
            {
                _hackerClient.Dispose();
                _hackerClient = null;
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
