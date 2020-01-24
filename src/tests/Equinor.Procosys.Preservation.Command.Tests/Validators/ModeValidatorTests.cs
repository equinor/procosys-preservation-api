using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.Mode;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class ModeValidatorTests
    {
        private const string ModeTitle = "ModeNotVoided";
        private const string ModeVoidedTitle = "ModeVoided";
        private const int ModeIdNonVoided = 1;
        private const int ModeIdVoided = 2;
        private ModeValidator _dut;

        [TestInitialize]
        public void Setup()
        {
            var modeRepositoryMock = new Mock<IModeRepository>();
            var journeyRepositoryMock = new Mock<IJourneyRepository>();
            journeyRepositoryMock.Setup(r => r.GetStepsByModeIdAsync(ModeIdNonVoided)).Returns(
                Task.FromResult(new List<Step>
                {
                    new Step("S", new Mock<Mode>().Object, new Mock<Responsible>().Object)
                }));

            var mode = new Mode("S", ModeTitle);
            var modeVoided = new Mode("S", ModeVoidedTitle);
            modeVoided.Void();

            modeRepositoryMock.Setup(r => r.GetByTitleAsync(ModeTitle)).Returns(Task.FromResult(mode));
            modeRepositoryMock.Setup(r => r.GetByIdAsync(ModeIdNonVoided)).Returns(Task.FromResult(mode));
            modeRepositoryMock.Setup(r => r.GetByIdAsync(ModeIdVoided)).Returns(Task.FromResult(modeVoided));
            modeRepositoryMock.Setup(r => r.Exists(ModeIdNonVoided)).Returns(Task.FromResult(true));

            _dut = new ModeValidator(modeRepositoryMock.Object, journeyRepositoryMock.Object);
        }

        [TestMethod]
        public void ValidateExists_KnownTitle_ReturnsTrue()
        {
            var result = _dut.Exists(ModeTitle);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateExists_KnownId_ReturnsTrue()
        {
            var result = _dut.Exists(ModeIdNonVoided);
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
            var result = _dut.IsVoided(ModeIdVoided);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateIsVoided_KnownNotVoided_ReturnsFalse()
        {
            var result = _dut.IsVoided(ModeIdNonVoided);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateIsVoided_UnknownId_ReturnsFalse()
        {
            var result = _dut.IsVoided(126234);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateIsUsedInStep_KnownId_ReturnsTrue()
        {
            var result = _dut.IsUsedInStep(ModeIdNonVoided);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateIsUsedInStep_UnknownId_ReturnsFalse()
        {
            var result = _dut.IsUsedInStep(126234);
            Assert.IsFalse(result);
        }
    }
}
