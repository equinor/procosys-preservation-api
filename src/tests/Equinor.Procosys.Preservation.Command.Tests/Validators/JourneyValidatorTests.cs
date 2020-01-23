using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.Journey;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class JourneyValidatorTests
    {
        private const string JourneyTitle = "J";
        private const int JourneyIdNonVoided = 1;
        private const int JourneyIdVoided = 2;
        private JourneyValidator _dut;

        [TestInitialize]
        public void Setup()
        {
            var journeyRepositoryMock = new Mock<IJourneyRepository>();

            var journey = new Journey("S", "J");
            var journeyVoided = new Journey("S", "J");
            journeyVoided.Void();

            journeyRepositoryMock.Setup(r => r.GetByTitleAsync(JourneyTitle)).Returns(Task.FromResult(journey));
            journeyRepositoryMock.Setup(r => r.GetByIdAsync(JourneyIdNonVoided)).Returns(Task.FromResult(journey));
            journeyRepositoryMock.Setup(r => r.GetByIdAsync(JourneyIdVoided)).Returns(Task.FromResult(journeyVoided));
            journeyRepositoryMock.Setup(r => r.Exists(JourneyIdNonVoided)).Returns(Task.FromResult(true));

            _dut = new JourneyValidator(journeyRepositoryMock.Object);
        }

        [TestMethod]
        public void ValidateExists_KnownTitle_ReturnsTrue()
        {
            var result = _dut.Exists(JourneyTitle);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateExists_KnownId_ReturnsTrue()
        {
            var result = _dut.Exists(JourneyIdNonVoided);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateExists_UnknownTitle_ReturnsFalse()
        {
            var result = _dut.Exists("XXX");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateExists_UnknownId_ReturnsFalse()
        {
            var result = _dut.Exists(126234);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateIsVoided_KnownVoided_ReturnsTrue()
        {
            var result = _dut.IsVoided(JourneyIdVoided);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateIsVoided_KnownNotVoided_ReturnsFalse()
        {
            var result = _dut.IsVoided(JourneyIdNonVoided);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateIsVoided_UnknownId_ReturnsFalse()
        {
            var result = _dut.IsVoided(126234);
            Assert.IsFalse(result);
        }
    }
}
