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
        private const string JourneyTitle1 = "Journey1";
        private const string JourneyTitle2 = "Journey2";

        private int _journeyWithStepId;
        private int _emptyJourneyId;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _journeyWithStepId = AddJourneyWithStep(context, JourneyTitle1, "S", AddMode(context, "M", false), AddResponsible(context, "R")).Id;
                _emptyJourneyId = AddJourney(context, JourneyTitle2).Id;
            }
        }
        
        [TestMethod]
        public async Task ExistsWithSameTitleAsync_KnownTitle_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsWithSameTitleAsync(JourneyTitle1, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownId_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsAsync(_journeyWithStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithSameTitleAsync_UnknownTitle_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsWithSameTitleAsync("XXX", default);
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
        public async Task ExistsWithSameTitleInAnotherJourneyAsync_SameTitleAsAnotherJourney_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsWithSameTitleInAnotherJourneyAsync(_journeyWithStepId, JourneyTitle2, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithSameTitleInAnotherJourneyAsync_NewTitle_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsWithSameTitleInAnotherJourneyAsync(_journeyWithStepId, "XXXXXX", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithSameTitleInAnotherJourneyAsync_SameTitle_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsWithSameTitleInAnotherJourneyAsync(_journeyWithStepId, JourneyTitle1, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownVoided_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var journey = context.Journeys.Single(j => j.Id == _journeyWithStepId);
                journey.Void();
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.IsVoidedAsync(_journeyWithStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownNotVoided_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.IsVoidedAsync(_journeyWithStepId, default);
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

        [TestMethod]
        public async Task HasAnyStepsAsync_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.HasAnyStepsAsync(_emptyJourneyId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HasAnyStepsAsync_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.HasAnyStepsAsync(_journeyWithStepId, default);
                Assert.IsTrue(result);
            }
        }
    }
}
