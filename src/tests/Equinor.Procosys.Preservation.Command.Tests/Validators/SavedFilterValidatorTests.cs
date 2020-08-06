using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.SavedFilterValidators;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Test.Common;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class SavedFilterValidatorTests : ReadOnlyTestsBase
    {
        private Mock<IPersonRepository> _personRepositoryMock;
        private Mock<ICurrentUserProvider> _currentUserProviderMock;
        private Guid _personOid;
        private SavedFilterValidator _dut;

        private readonly string _title = "title";

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                const string Criteria = "criteria";
                _personOid = new Guid();

                var person = AddPerson(context, _personOid, "Current", "User");
                var savedFilter = new SavedFilter(TestPlant, _title, Criteria, false);
                
                person.AddSavedFilter(savedFilter);
                context.SaveChangesAsync().Wait();

                _personRepositoryMock = new Mock<IPersonRepository>();
                _personRepositoryMock
                    .Setup(p => p.GetByOidAsync(It.Is<Guid>(x => x == _personOid)))
                    .Returns(Task.FromResult(person));

                _currentUserProviderMock = new Mock<ICurrentUserProvider>();
                _currentUserProviderMock
                    .Setup(x => x.GetCurrentUserOid())
                    .Returns(_personOid);
            }
        }

        [TestMethod]
        public async Task IsValid_UnknownTitle_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                _dut = new SavedFilterValidator(context, _personRepositoryMock.Object, _currentUserProviderMock.Object);
                var result = await _dut.ExistsWithSameTitleForPersonAsync("xxx", default);

                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsValid_KnownTitle_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                _dut = new SavedFilterValidator(context, _personRepositoryMock.Object, _currentUserProviderMock.Object);
                var result = await _dut.ExistsWithSameTitleForPersonAsync(_title, default);

                Assert.IsTrue(result);
            }
        }
    }
}
