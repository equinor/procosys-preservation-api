using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.BlobStorage;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.MainApi.Area;
using Equinor.Procosys.Preservation.MainApi.Discipline;
using Equinor.Procosys.Preservation.MainApi.Permission;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Equinor.Procosys.Preservation.MainApi.Project;
using Equinor.Procosys.Preservation.MainApi.Responsible;
using Equinor.Procosys.Preservation.WebApi.Authorizations;
using Equinor.Procosys.Preservation.WebApi.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using ProcosysProject = Equinor.Procosys.Preservation.MainApi.Permission.ProcosysProject;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests
{
    public sealed class TestFactory : WebApplicationFactory<Startup>
    {
        private readonly string _libraryAdminOid = "00000000-0000-0000-0000-000000000001";
        private readonly string _plannerOid = "00000000-0000-0000-0000-000000000002";
        private readonly string _preserverOid = "00000000-0000-0000-0000-000000000003";
        private readonly string _hackerOid = "00000000-0000-0000-0000-000000000666";
        private readonly string _integrationTestEnvironment = "IntegrationTests";
        private readonly string _connectionString;
        private readonly string _configPath;
        private readonly Dictionary<UserType, ITestUser> _testUsers = new Dictionary<UserType, ITestUser>();
        private readonly List<Action> _teardownList = new List<Action>();
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        private readonly Mock<IPlantApiService> _plantApiServiceMock = new Mock<IPlantApiService>();
        private readonly Mock<IProjectApiService> _projectApiServiceMock = new Mock<IProjectApiService>();
        private readonly Mock<IPermissionApiService> _permissionApiServiceMock = new Mock<IPermissionApiService>();
        public readonly Mock<IResponsibleApiService> ResponsibleApiServiceMock = new Mock<IResponsibleApiService>();
        public readonly Mock<IDisciplineApiService> DisciplineApiServiceMock = new Mock<IDisciplineApiService>();
        public readonly Mock<IAreaApiService> AreaApiServiceMock = new Mock<IAreaApiService>();
        public readonly Mock<IBlobStorage> BlobStorageMock = new Mock<IBlobStorage>();

        public static string PlantWithAccess => KnownTestData.Plant;
        public static string PlantWithoutAccess => "PCS$PLANT999";
        public static string UnknownPlant => "UNKNOWN_PLANT";
        public static string ProjectWithAccess => KnownTestData.ProjectName;
        public static string ProjectWithoutAccess => "Project999";
        public static string AValidRowVersion => "AAAAAAAAAAA=";

        public KnownTestData KnownTestData { get; }

        #region singleton implementation
        private static TestFactory s_instance;
        private static readonly object s_padlock = new object();

        public static TestFactory Instance
        {
            get
            {
                if (s_instance == null)
                {
                    lock (s_padlock)
                    {
                        if (s_instance == null)
                        {
                            s_instance = new TestFactory();
                        }
                    }
                }

                return s_instance;
            }
        }

        private TestFactory()
        {
            KnownTestData = new KnownTestData();

            var projectDir = Directory.GetCurrentDirectory();
            _connectionString = GetTestDbConnectionString(projectDir);
            _configPath = Path.Combine(projectDir, "appsettings.json");

            SetupTestUsers();
        }
        #endregion

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

        public HttpClient GetHttpClient(UserType userType, string plant)
        {
            var testUser = _testUsers[userType];
            
            // Need to change what the mock returns each time since the factory share the same registered mocks
            SetupPlantMock(testUser.ProCoSysPlants);
            
            SetupPermissionMock(plant, 
                testUser.ProCoSysPermissions,
                testUser.ProCoSysProjects,
                testUser.ProCoSysRestrictions);
            
            UpdatePlantInHeader(testUser.HttpClient, plant);
            
            return testUser.HttpClient;
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
                services.AddScoped(serviceProvider => _projectApiServiceMock.Object);
                services.AddScoped(serviceProvider => _permissionApiServiceMock.Object);
                services.AddScoped(serviceProvider => ResponsibleApiServiceMock.Object);
                services.AddScoped(serviceProvider => DisciplineApiServiceMock.Object);
                services.AddScoped(serviceProvider => AreaApiServiceMock.Object);
                services.AddScoped(serviceProvider => BlobStorageMock.Object);
            });

            builder.ConfigureServices(services =>
            {
                ReplaceRealDbContextWithTestDbContext(services);
                
                CreateSeededTestDatabase(services);
                
                EnsureTestDatabaseDeletedAtTeardown(services);
            });
        }

        private void ReplaceRealDbContextWithTestDbContext(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault
                (d => d.ServiceType == typeof(DbContextOptions<PreservationContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<PreservationContext>(options => options.UseSqlServer(_connectionString));
        }

        private void CreateSeededTestDatabase(IServiceCollection services)
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<PreservationContext>();

                    dbContext.Database.EnsureDeleted();

                    dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));

                    dbContext.CreateNewDatabaseWithCorrectSchema();
                    var migrations = dbContext.Database.GetPendingMigrations();
                    if (migrations.Any())
                    {
                        dbContext.Database.Migrate();
                    }

                    dbContext.Seed(scope.ServiceProvider, KnownTestData);
                }
            }
        }

        private void EnsureTestDatabaseDeletedAtTeardown(IServiceCollection services)
            => _teardownList.Add(() =>
            {
                using (var dbContext = DatabaseContext(services))
                {
                    dbContext.Database.EnsureDeleted();
                }
            });

        private PreservationContext DatabaseContext(IServiceCollection services)
        {
            services.AddDbContext<PreservationContext>(options => options.UseSqlServer(_connectionString));

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
            
            // Set Initial Catalog to be able to delete database!
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

            var commonProCoSysRestrictions = new List<string>
            {
                ClaimsTransformation.NoRestrictions
            };

            AddAnonymousUser();

            AddLibraryAdminUser(commonProCoSysPlants, commonProCoSysProjects, commonProCoSysRestrictions);

            AddPlannerUser(commonProCoSysPlants, commonProCoSysProjects, commonProCoSysRestrictions);

            AddPreserverUser(commonProCoSysPlants, commonProCoSysProjects, commonProCoSysRestrictions);
    
            AddHackerUser(commonProCoSysProjects);
            
            var webHostBuilder = WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment(_integrationTestEnvironment);
                builder.ConfigureAppConfiguration((context, conf) => conf.AddJsonFile(_configPath));
            });

            foreach (var testUser in _testUsers.Values)
            {
                testUser.HttpClient = webHostBuilder.CreateClient();

                if (testUser.Profile != null)
                {
                    AuthenticateUser(testUser);
                }
            }
        }

        // Authenticated client without any roles
        private void AddHackerUser(List<ProcosysProject> commonProCoSysProjects)
            => _testUsers.Add(UserType.Hacker,
                new TestUser
                {
                    Profile =
                        new TestProfile
                        {
                            FullName = "Harry Hacker", 
                            Oid = _hackerOid
                        },
                    ProCoSysPlants = new List<ProcosysPlant>
                    {
                        new ProcosysPlant {Id = PlantWithAccess},
                        new ProcosysPlant {Id = PlantWithoutAccess}
                    },
                    ProCoSysPermissions = new List<string>(),
                    ProCoSysProjects = commonProCoSysProjects,
                    ProCoSysRestrictions = new List<string>()
                });

        // Authenticated client with necessary roles to perform preservation work
        private void AddPreserverUser(
            List<ProcosysPlant> commonProCoSysPlants,
            List<ProcosysProject> commonProCoSysProjects,
            List<string> commonProCoSysRestrictions)
            => _testUsers.Add(UserType.Preserver,
                new TestUser
                {
                    Profile =
                        new TestProfile
                        {
                            FullName = "Peder Preserver",
                            Oid = _preserverOid
                        },
                    ProCoSysPlants = commonProCoSysPlants,
                    ProCoSysPermissions = new List<string>
                    {
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

        // Authenticated user with necessary roles to Create and Update in Scope
        private void AddPlannerUser(
            List<ProcosysPlant> commonProCoSysPlants,
            List<ProcosysProject> commonProCoSysProjects,
            List<string> commonProCoSysRestrictions)
            => _testUsers.Add(UserType.Planner,
                new TestUser
                {
                    Profile =
                        new TestProfile
                        {
                            FullName = "Pernilla Planner",
                            Oid = _plannerOid
                        },
                    ProCoSysPlants = commonProCoSysPlants,
                    ProCoSysPermissions = new List<string>
                    {
                        Permissions.PRESERVATION_PLAN_READ,
                        Permissions.PRESERVATION_PLAN_CREATE,
                        Permissions.PRESERVATION_PLAN_DELETE,
                        Permissions.PRESERVATION_PLAN_VOIDUNVOID,
                        Permissions.PRESERVATION_PLAN_WRITE,
                        Permissions.PRESERVATION_PLAN_ATTACHFILE,
                        Permissions.PRESERVATION_PLAN_DETACHFILE
                    },
                    ProCoSysProjects = commonProCoSysProjects,
                    ProCoSysRestrictions = commonProCoSysRestrictions
                });

        // Authenticated client with necessary roles to Create and Update in Library
        private void AddLibraryAdminUser(
            List<ProcosysPlant> commonProCoSysPlants,
            List<ProcosysProject> commonProCoSysProjects,
            List<string> commonProCoSysRestrictions)
            => _testUsers.Add(UserType.LibraryAdmin,
                new TestUser
                {
                    Profile =
                        new TestProfile
                        {
                            FullName = "Arne Admin",
                            Oid = _libraryAdminOid
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

        private void AddAnonymousUser() => _testUsers.Add(UserType.Anonymous, new TestUser());

        private void AuthenticateUser(ITestUser user)
            => user.HttpClient.DefaultRequestHeaders.Add("Authorization", CreateBearerToken(user.Profile));

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
        
        /// <summary>
        /// Wraps profile by serializing, encoding and then converting to base 64 string.
        /// "Bearer" is also added, making it ready to be added as Authorization header
        /// </summary>
        /// <param name="profile">The instance of the token to be wrapped</param>
        /// <returns>Serialized, encoded string ready for authorization header</returns>
        private string CreateBearerToken(TestProfile profile)
        {
            var serialized = JsonConvert.SerializeObject(profile);
            var tokenBytes = Encoding.UTF8.GetBytes(serialized);
            var tokenString = Convert.ToBase64String(tokenBytes);

            return $"Bearer {tokenString}";
        }
    }
}
