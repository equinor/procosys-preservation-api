using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class JourneyValidatorTests : ReadOnlyTestsBase
    {
        private const string JourneyTitle = "Journey";
        private const string JourneyTitle2 = "Journey2";

        private int _journeyId;
        private int _journeyId2;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _journeyId = AddJourneyWithStep(context, JourneyTitle, "S", AddMode(context, "M"), AddResponsible(context, "R")).Id;
                _journeyId2 = AddJourneyWithStep(context, JourneyTitle2, "S", AddMode(context, "M2"), AddResponsible(context, "R2")).Id;
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownTitle_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsAsync(JourneyTitle, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownId_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsAsync(_journeyId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownTitle_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsAsync("XXX", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_SameTitleAsAnotherJourney_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsAsync(_journeyId, JourneyTitle2, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_NewTitle_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsAsync(_journeyId, "XXXXXX", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_SameTitle_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsAsync(_journeyId2, JourneyTitle2, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownVoided_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var journey = context.Journeys.Single(j => j.Id == _journeyId);
                journey.Void();
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.IsVoidedAsync(_journeyId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownNotVoided_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.IsVoidedAsync(_journeyId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.IsVoidedAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

    }
}
