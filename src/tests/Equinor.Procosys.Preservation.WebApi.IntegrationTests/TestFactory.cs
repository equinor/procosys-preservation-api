using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.MainApi.Permission;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Equinor.Procosys.Preservation.WebApi.IntegrationTests.Clients;
using Equinor.Procosys.Preservation.WebApi.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests
{
    public class TestFactory : WebApplicationFactory<Startup>
    {
        private readonly Mock<IPlantApiService> _plantApiServiceMock;
        private readonly Mock<IPermissionApiService> _permissionApiServiceMock;
        private readonly string _connectionString;
        private readonly string _configPath;
        private Dictionary<string, ITestUser> _testUsers = new Dictionary<string, ITestUser>();
        private readonly List<Action> _teardownList = new List<Action>();
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        public static string AnonymousUser = "NN";
        public static string LibraryAdminUser = "Arne Admin";
        public static string PlannerUser = "Pernilla Planner";
        public static string PreserverUser = "Peder Preserver";
        public static string HackerUser = "Harry Hacker";
        public static string PlantWithAccess => "PLANT1";
        public static string PlantWithoutAccess => "PLANT999";
        public static string UnknownPlant => "UNKNOWN_PLANT";
        public static string ProjectWithAccess => "Project1";
        public static string ProjectWithoutAccess => "Project999";
        public static string AValidRowVersion => "AAAAAAAAAAA=";

        public TestFactory()
        {
            var projectDir = Directory.GetCurrentDirectory();
            _connectionString = GetTestDbConnectionString(projectDir);
            _configPath = Path.Combine(projectDir, "appsettings.json");

            _plantApiServiceMock = new Mock<IPlantApiService>();

            _permissionApiServiceMock = new Mock<IPermissionApiService>();

            SetupTestUsers();
        }

        public HttpClient GetClientForPlant(string user, string plant)
        {
            var testUser = _testUsers[user];
            
            // Need to change what the mock returns each time since the factory share the same registered mocks
            SetupPlantMock(testUser.ProCoSysPlants);
            
            SetupPermissionMock(plant, 
                testUser.ProCoSysPermissions,
                testUser.ProCoSysProjects,
                testUser.ProCoSysRestrictions);
            
            UpdatePlantInHeader(testUser.HttpClient, plant);
            
            return testUser.HttpClient;
        }

        public new void Dispose()
        {
            // Run teardown
            foreach (var action in _teardownList)
            {
                action();
            }

            foreach (var testUser in _testUsers)
            {
                testUser.Value.HttpClient.Dispose();
            }
            
            foreach (var disposable in _disposables)
            {
                try { disposable.Dispose(); } catch { /* Ignore */ }
            }

            base.Dispose();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication()
                    .AddScheme<IntegrationTestAuthOptions, IntegrationTestAuthHandler>(
                        IntegrationTestAuthHandler.TestAuthenticationScheme, opts => { });

                services.PostConfigureAll<JwtBearerOptions>(jwtBearerOptions =>
                    jwtBearerOptions.ForwardAuthenticate = IntegrationTestAuthHandler.TestAuthenticationScheme);

                services.AddScoped(serviceProvider => _plantApiServiceMock.Object);
                services.AddScoped(serviceProvider => _permissionApiServiceMock.Object);
            });

            builder.ConfigureServices(CreateDatabaseWithMigrations);
        }

        private void CreateDatabaseWithMigrations(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault
                (d => d.ServiceType == typeof(DbContextOptions<PreservationContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<PreservationContext>(context => context.UseSqlServer(_connectionString));

            using var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<PreservationContext>();

            dbContext.Database.EnsureDeleted();

            dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));

            var migrations = dbContext.Database.GetPendingMigrations();
            if (migrations.Any())
            {
                dbContext.Database.Migrate();
            }

            // Put the teardown here, as we don't have the generic TContext in the dispose method.
            _teardownList.Add(() =>
            {
                using var dbContextForTeardown = DatabaseContext(services);

                dbContextForTeardown.Database.EnsureDeleted();
            });
        }
        private PreservationContext DatabaseContext(IServiceCollection services)
        {
            services.AddDbContext<PreservationContext>(options =>
            {
                options.UseSqlServer(_connectionString);
            });

            var sp = services.BuildServiceProvider();
            _disposables.Add(sp);

            var spScope = sp.CreateScope();
            _disposables.Add(spScope);

            return spScope.ServiceProvider.GetRequiredService<PreservationContext>();
        }

        private string GetTestDbConnectionString(string projectDir)
        {
            var dbName = "IntegrationTestsDB";
            var dbPath = Path.Combine(projectDir, $"{dbName}.mdf");
            return $"Server=(LocalDB)\\MSSQLLocalDB;Initial Catalog={dbName};Integrated Security=true;AttachDbFileName={dbPath}";
        }

        private void SetupPlantMock(List<ProcosysPlant> plants)
            => _plantApiServiceMock.Setup(p => p.GetAllPlantsAsync()).Returns(Task.FromResult(plants));
        
        private void SetupPermissionMock(
            string plant,
            IList<string> proCoSysPermissions,
            IList<ProcosysProject> proCoSysProjects,
            IList<string> proCoSysRestrictions
            )
        {
            _permissionApiServiceMock.Setup(p => p.GetPermissionsAsync(plant))
                .Returns(Task.FromResult(proCoSysPermissions));
                        
            _permissionApiServiceMock.Setup(p => p.GetAllOpenProjectsAsync(plant))
                .Returns(Task.FromResult(proCoSysProjects));

            _permissionApiServiceMock.Setup(p => p.GetContentRestrictionsAsync(plant))
                .Returns(Task.FromResult(proCoSysRestrictions));
        }

        private void SetupTestUsers()
        {
            var commonProCoSysPlants = new List<ProcosysPlant>
            {
                new ProcosysPlant {Id = PlantWithAccess, HasAccess = true},
                new ProcosysPlant {Id = PlantWithoutAccess}
            };

            var commonProCoSysProjects = new List<ProcosysProject>
            {
                new ProcosysProject {Name = ProjectWithAccess, HasAccess = true},
                new ProcosysProject {Name = ProjectWithoutAccess}
            };

            var commonProCoSysRestrictions = new List<string>();

            _testUsers.Add(AnonymousUser, new TestUser());

            // Authenticated client with necessary roles to Create and Update in Library
            _testUsers.Add(LibraryAdminUser,
                new TestUser
                {
                    Profile =
                        new TestProfile
                        {
                            FullName = LibraryAdminUser,
                            Oid = "00000000-0000-0000-0000-000000000001"
                        },
                    ProCoSysPlants = commonProCoSysPlants,
                    ProCoSysPermissions = new List<string>
                    {
                        Permissions.LIBRARY_PRESERVATION_CREATE,
                        Permissions.LIBRARY_PRESERVATION_DELETE,
                        Permissions.LIBRARY_PRESERVATION_READ,
                        Permissions.LIBRARY_PRESERVATION_VOIDUNVOID,
                        Permissions.LIBRARY_PRESERVATION_WRITE
                    },
                    ProCoSysProjects = commonProCoSysProjects,
                    ProCoSysRestrictions = commonProCoSysRestrictions
                });

            // Authenticated client with necessary roles to Create and Update in Scope
            _testUsers.Add(PlannerUser,
                new TestUser
                {
                    Profile =
                        new TestProfile
                        {
                            FullName = PlannerUser,
                            Oid = "00000000-0000-0000-0000-000000000002"
                        },
                    ProCoSysPlants = commonProCoSysPlants,
                    ProCoSysPermissions = new List<string>
                    {
                        Permissions.LIBRARY_PRESERVATION_READ,
                        Permissions.PRESERVATION_PLAN_CREATE,
                        Permissions.PRESERVATION_PLAN_DELETE,
                        Permissions.PRESERVATION_PLAN_VOIDUNVOID,
                        Permissions.PRESERVATION_PLAN_WRITE
                    },
                    ProCoSysProjects = commonProCoSysProjects,
                    ProCoSysRestrictions = commonProCoSysRestrictions
                });

            // Authenticated client with necessary roles to perform preservation work
            _testUsers.Add(PreserverUser,
                new TestUser
                {
                    Profile =
                        new TestProfile
                        {
                            FullName = PreserverUser,
                            Oid = "00000000-0000-0000-0000-000000000003"
                        },
                    ProCoSysPlants = commonProCoSysPlants,
                    ProCoSysPermissions = new List<string>
                    {
                        Permissions.LIBRARY_PRESERVATION_READ,
                        Permissions.PRESERVATION_CREATE,
                        Permissions.PRESERVATION_DELETE,
                        Permissions.PRESERVATION_READ,
                        Permissions.PRESERVATION_WRITE,
                        Permissions.PRESERVATION_ATTACHFILE,
                        Permissions.PRESERVATION_DETACHFILE
                    },
                    ProCoSysProjects = commonProCoSysProjects,
                    ProCoSysRestrictions = commonProCoSysRestrictions
                });
    
            // Authenticated client without any roles
            _testUsers.Add(HackerUser,
                new TestUser
                {
                    Profile =
                        new TestProfile
                        {
                            FullName = HackerUser,
                            Oid = "00000000-0000-0000-0000-000000000666"
                        },
                    ProCoSysPlants = new List<ProcosysPlant>
                    {
                        new ProcosysPlant {Id = PlantWithAccess},
                        new ProcosysPlant {Id = PlantWithoutAccess}
                    },
                    ProCoSysPermissions = new List<string>(),
                    ProCoSysProjects = commonProCoSysProjects,
                    ProCoSysRestrictions = commonProCoSysRestrictions
                });
            
            foreach (var testUser in _testUsers)
            {
                SetupHttpClient(testUser.Value);
            }
        }

        private void SetupHttpClient(ITestUser user)
        {
            user.HttpClient = WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment(Startup.IntegrationTestEnvironment);
                builder.ConfigureAppConfiguration((context, conf) => conf.AddJsonFile(_configPath));

            }).CreateClient();

            if (user.Profile != null)
            {
                user.HttpClient.DefaultRequestHeaders.Add("Authorization", BearerTokenUtility.WrapAuthToken(user.Profile));
            }
        }

        private void UpdatePlantInHeader(HttpClient client, string plant)
        {
            if (client.DefaultRequestHeaders.Contains(CurrentPlantMiddleware.PlantHeader))
            {
                client.DefaultRequestHeaders.Remove(CurrentPlantMiddleware.PlantHeader);
            }

            if (!string.IsNullOrEmpty(plant))
            {
                client.DefaultRequestHeaders.Add(CurrentPlantMiddleware.PlantHeader, plant);
            }
        }
    }
}
