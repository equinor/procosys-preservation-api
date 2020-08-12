using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetSavedFiltersInProject;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.GetSavedFiltersInProject
{
    [TestClass]
    public class GetSavedFiltersInProjectQueryHandlerTests : ReadOnlyTestsBase
    {
        private GetSavedFiltersInProjectQuery _query;
        private Mock<ICurrentUserProvider> _currentUserProviderMock;

        private const string _title = "title";
        private const string _criteria = "criteria in JSON";
        private const string _projectName = "projectName";
        private bool _defaultFilter = true;

        private SavedFilter _savedFilter;
        private Person _person;
        private Project _project;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                _query = new GetSavedFiltersInProjectQuery(_projectName);

                _project = AddProject(context, _projectName, "", true);

                _savedFilter = new SavedFilter(TestPlant, _project, _title, _criteria)
                    { DefaultFilter =  _defaultFilter };
                _person = AddPerson(context, Guid.NewGuid(), "FistName", "LastName");
                _person.AddSavedFilter(_savedFilter);

                _currentUserProviderMock = new Mock<ICurrentUserProvider>();
                _currentUserProviderMock
                    .Setup(x => x.GetCurrentUserOid())
                    .Returns(_person.Oid);

                context.SaveChangesAsync().Wait();
            }
        }

        [TestMethod]
        public async Task HandleGetSavedFiltersInProjectQuery_ShouldReturnOkResult()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var dut = new GetSavedFiltersInProjectQueryHandler(context, _currentUserProviderMock.Object);
                var result = await dut.Handle(_query, default);

                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestMethod]
        public async Task HandleGetSavedFiltersInProjectQuery_ShouldReturnCorrectSavedFilters()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var dut = new GetSavedFiltersInProjectQueryHandler(context, _currentUserProviderMock.Object);

                var result = await dut.Handle(_query, default);
                var savedFilter = result.Data.Single();

                Assert.AreEqual(1, result.Data.Count);
                Assert.AreEqual(_title, savedFilter.Title);
                Assert.AreEqual(_criteria, savedFilter.Criteria);
                Assert.AreEqual(_defaultFilter, savedFilter.DefaultFilter);
            }
        }

        [TestMethod]
        public async Task HandleGetSavedFiltersInProjectQuery_ShouldReturnEmptyListOfSavedFilters()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var dut = new GetSavedFiltersInProjectQueryHandler(context, _currentUserProvider);

                var result = await dut.Handle(new GetSavedFiltersInProjectQuery(_projectName), default);
                Assert.AreEqual(0, result.Data.Count);
            }
        }
    }
}
