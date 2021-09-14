using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.BlobStorage;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.MainApi.Area;
using Equinor.ProCoSys.Preservation.MainApi.Discipline;
using Equinor.ProCoSys.Preservation.MainApi.Permission;
using Equinor.ProCoSys.Preservation.MainApi.Person;
using Equinor.ProCoSys.Preservation.MainApi.Plant;
using Equinor.ProCoSys.Preservation.MainApi.Project;
using Equinor.ProCoSys.Preservation.MainApi.Responsible;
using Equinor.ProCoSys.Preservation.MainApi.Tag;
using Equinor.ProCoSys.Preservation.WebApi.Authorizations;
using Equinor.ProCoSys.Preservation.WebApi.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PCSProject = Equinor.ProCoSys.Preservation.MainApi.Permission.PCSProject;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests
{
    public sealed class TestFactory : WebApplicationFactory<Startup>
    {
        private readonly string _libraryAdminOid = "00000000-0000-0000-0000-000000000001";
        private readonly string _crossPlantAppOid = "00000000-0000-0000-0000-000000000004";
        private readonly string _plannerOid = "00000000-0000-0000-0000-000000000002";
        private readonly string _preserverOid = "00000000-0000-0000-0000-000000000003";
        private readonly string _hackerOid = "00000000-0000-0000-0000-000000000666";
        private readonly string _integrationTestEnvironment = "IntegrationTests";
        private readonly string _connectionString;
        private readonly string _configPath;
        private readonly Dictionary<UserType, ITestUser> _testUsers = new Dictionary<UserType, ITestUser>();
        private readonly List<Action> _teardownList = new List<Action>();
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        private readonly Mock<IPersonApiService> _personApiServiceMock = new Mock<IPersonApiService>();
        private readonly Mock<IPlantApiService> _plantApiServiceMock = new Mock<IPlantApiService>();
        private readonly Mock<IProjectApiService> _projectApiServiceMock = new Mock<IProjectApiService>();
        private readonly Mock<IPermissionApiService> _permissionApiServiceMock = new Mock<IPermissionApiService>();
        public readonly Mock<IResponsibleApiService> ResponsibleApiServiceMock = new Mock<IResponsibleApiService>();
        public readonly Mock<IDisciplineApiService> DisciplineApiServiceMock = new Mock<IDisciplineApiService>();
        public readonly Mock<IAreaApiService> AreaApiServiceMock = new Mock<IAreaApiService>();
        public readonly Mock<IBlobStorage> BlobStorageMock = new Mock<IBlobStorage>();
        public readonly Mock<ITagApiService> TagApiServiceMock = new Mock<ITagApiService>();

        public static string PlantWithAccess => KnownPlantData.PlantA;
        public static string PlantWithoutAccess => KnownPlantData.PlantB;
        public static string UnknownPlant => "UNKNOWN_PLANT";
        public static string ProjectWithAccess => KnownTestData.ProjectName;
        public static string ProjectWithoutAccess => "Project999";
        public static string AValidRowVersion => "AAAAAAAAAAA=";

        public Dictionary<string, KnownTestData> SeededData { get; }

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
            SeededData = new Dictionary<string, KnownTestData>();

            var projectDir = Directory.GetCurrentDirectory();
            _connectionString = GetTestDbConnectionString(projectDir);
            _configPath = Path.Combine(projectDir, "appsettings.json");

            SetupPreservationApiUser();

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
            
            lock (s_padlock)
            {
                s_instance = null;
            }

            base.Dispose();
        }

        public HttpClient GetHttpClient(UserType userType, string plant)
        {
            var testUser = _testUsers[userType];
            
            SetupPermissionMock(plant, 
                testUser.ProCoSysPermissions,
                testUser.ProCoSysProjects,
                testUser.ProCoSysRestrictions);
            
            UpdatePlantInHeader(testUser.HttpClient, plant);
            
            return testUser.HttpClient;
        }

        public TestProfile GetTestProfile(UserType userType) => _testUsers[userType].Profile;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication()
                    .AddScheme<IntegrationTestAuthOptions, IntegrationTestAuthHandler>(
                        IntegrationTestAuthHandler.TestAuthenticationScheme, _ => { });

                services.PostConfigureAll<JwtBearerOptions>(jwtBearerOptions =>
                    jwtBearerOptions.ForwardAuthenticate = IntegrationTestAuthHandler.TestAuthenticationScheme);

                services.AddScoped(_ => _personApiServiceMock.Object);
                services.AddScoped(_ => _plantApiServiceMock.Object);
                services.AddScoped(_ => _projectApiServiceMock.Object);
                services.AddScoped(_ => TagApiServiceMock.Object);
                services.AddScoped(_ => _permissionApiServiceMock.Object);
                services.AddScoped(_ => ResponsibleApiServiceMock.Object);
                services.AddScoped(_ => DisciplineApiServiceMock.Object);
                services.AddScoped(_ => AreaApiServiceMock.Object);
                services.AddScoped(_ => BlobStorageMock.Object);
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
                    var scopeServiceProvider = scope.ServiceProvider;
                    var dbContext = scopeServiceProvider.GetRequiredService<PreservationContext>();

                    dbContext.Database.EnsureDeleted();

                    dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));

                    dbContext.CreateNewDatabaseWithCorrectSchema();
                    var migrations = dbContext.Database.GetPendingMigrations();
                    if (migrations.Any())
                    {
                        dbContext.Database.Migrate();
                    }

                    SeedDataForPlant(dbContext, scopeServiceProvider, KnownPlantData.PlantA);
                    SeedDataForPlant(dbContext, scopeServiceProvider, KnownPlantData.PlantB);
                }
            }
        }

        private void SeedDataForPlant(PreservationContext dbContext, IServiceProvider scopeServiceProvider, string plant)
        {
            var knownData = new KnownTestData(plant);
            SeededData.Add(plant, knownData);
            dbContext.Seed(scopeServiceProvider, knownData);
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
            var dbName = "IntegrationTestsPresDB";
            var dbPath = Path.Combine(projectDir, $"{dbName}.mdf");
            
            // Set Initial Catalog to be able to delete database!
            return $"Server=(LocalDB)\\MSSQLLocalDB;Initial Catalog={dbName};Integrated Security=true;AttachDbFileName={dbPath}";
        }
        
        private void SetupPermissionMock(
            string plant,
            IList<string> proCoSysPermissions,
            IList<PCSProject> proCoSysProjects,
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
            var commonProCoSysPlants = new List<PCSPlant>
            {
                new PCSPlant {Id = KnownPlantData.PlantA, Title = KnownPlantData.PlantATitle, HasAccess = true},
                new PCSPlant {Id = KnownPlantData.PlantB, Title = KnownPlantData.PlantBTitle}
            };

            var commonProCoSysProjects = new List<PCSProject>
            {
                new PCSProject {Name = ProjectWithAccess, HasAccess = true},
                new PCSProject {Name = ProjectWithoutAccess}
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
            
            AddCrossPlantApp();
            
            var webHostBuilder = WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment(_integrationTestEnvironment);
                builder.ConfigureAppConfiguration((_, conf) => conf.AddJsonFile(_configPath));
            });

            foreach (var testUser in _testUsers.Values)
            {
                testUser.HttpClient = webHostBuilder.CreateClient();

                if (testUser.Profile != null)
                {
                    AuthenticateUser(testUser);

                    _personApiServiceMock.Setup(p => p.TryGetPersonByOidAsync(testUser.Profile.AzureOid))
                        .Returns(Task.FromResult( new PCSPerson
                        {
                            AzureOid = testUser.Profile.Oid,
                            FirstName = testUser.Profile.FirstName,
                            LastName = testUser.Profile.LastName
                        }));

                    _plantApiServiceMock.Setup(p => p.GetAllPlantsForUserAsync(testUser.Profile.AzureOid))
                        .Returns(Task.FromResult(testUser.ProCoSysPlants));
                }
            }
        }
        
        private void SetupPreservationApiUser()
        {
            var config = new ConfigurationBuilder().AddJsonFile(_configPath).Build();

            var preservationApiObjectId = new Guid(config["Authenticator:PreservationApiObjectId"]);
            _personApiServiceMock.Setup(p => p.TryGetPersonByOidAsync(preservationApiObjectId))
                .Returns(Task.FromResult( new PCSPerson
                {
                    AzureOid = preservationApiObjectId.ToString("N"),
                    FirstName = "Preservation",
                    LastName = "API App"
                }));
        }

        // Authenticated client without any roles
        private void AddHackerUser(List<PCSProject> commonProCoSysProjects)
            => _testUsers.Add(UserType.Hacker,
                new TestUser
                {
                    Profile =
                        new TestProfile
                        {
                            FirstName = "Harry",
                            LastName = "Hacker", 
                            Oid = _hackerOid
                        },
                    ProCoSysPlants = new List<PCSPlant>
                    {
                        new PCSPlant {Id = KnownPlantData.PlantA, Title = KnownPlantData.PlantATitle},
                        new PCSPlant {Id = KnownPlantData.PlantB, Title = KnownPlantData.PlantBTitle}
                    },
                    ProCoSysPermissions = new List<string>(),
                    ProCoSysProjects = commonProCoSysProjects,
                    ProCoSysRestrictions = new List<string>()
                });

        // Authenticated client with necessary roles to perform preservation work
        private void AddPreserverUser(
            List<PCSPlant> commonProCoSysPlants,
            List<PCSProject> commonProCoSysProjects,
            List<string> commonProCoSysRestrictions)
            => _testUsers.Add(UserType.Preserver,
                new TestUser
                {
                    Profile =
                        new TestProfile
                        {
                            FirstName = "Peder",
                            LastName = "Preserver",
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
            List<PCSPlant> commonProCoSysPlants,
            List<PCSProject> commonProCoSysProjects,
            List<string> commonProCoSysRestrictions)
            => _testUsers.Add(UserType.Planner,
                new TestUser
                {
                    Profile =
                        new TestProfile
                        {
                            FirstName = "Pernilla",
                            LastName = "Planner",
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
            List<PCSPlant> commonProCoSysPlants,
            List<PCSProject> commonProCoSysProjects,
            List<string> commonProCoSysRestrictions)
            => _testUsers.Add(UserType.LibraryAdmin,
                new TestUser
                {
                    Profile =
                        new TestProfile
                        {
                            FirstName = "Arne",
                            LastName = "Admin",
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
        
        // Authenticated client without any roles. Configured in app config to allow using cross plant endpoints
        private void AddCrossPlantApp()
            => _testUsers.Add(UserType.CrossPlantApp,
                new TestUser
                {
                    Profile =
                        new TestProfile
                        {
                            FirstName = "XPlant",
                            LastName = "App",
                            Oid = _crossPlantAppOid,
                            IsAppToken = true,
                            AppRoles = new []{AppRoles.CROSSPLANT}
                        },
                    ProCoSysPlants = new List<PCSPlant>
                    {
                        new PCSPlant {Id = KnownPlantData.PlantA, Title = KnownPlantData.PlantATitle},
                        new PCSPlant {Id = KnownPlantData.PlantB, Title = KnownPlantData.PlantBTitle}
                    },
                    ProCoSysPermissions = new List<string>(),
                    ProCoSysProjects = new List<PCSProject>(),
                    ProCoSysRestrictions = new List<string>()
                });

        private void AddAnonymousUser() => _testUsers.Add(UserType.Anonymous, new TestUser());

        private void AuthenticateUser(ITestUser user)
            => user.HttpClient.DefaultRequestHeaders.Add("Authorization", user.Profile.CreateBearerToken());

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
