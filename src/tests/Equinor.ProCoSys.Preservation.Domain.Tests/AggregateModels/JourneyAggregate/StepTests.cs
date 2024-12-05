using System;
using System.Linq;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.Test.Common;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.JourneyAggregate
{
    [TestClass]
    public class StepTests
    {
        private const string TestPlant = "PlantA";
        private Mode _mode;
        private Responsible _responsible;
        private Step _dut;

        [TestInitialize]
        public void Setup()
        {
            var utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            var timeProvider = new ManualTimeProvider(utcNow);
            TimeService.SetProvider(timeProvider);
            
            _mode = new Mode(TestPlant, "SUP", true);
            _mode.SetProtectedIdForTesting(3);

            _responsible = new Responsible(TestPlant, "RC", "RD");
            _responsible.SetProtectedIdForTesting(4);

            _dut = new Step(TestPlant, "S", _mode, _responsible);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            Assert.AreEqual(TestPlant, _dut.Plant);
            Assert.AreEqual("S", _dut.Title);
            Assert.AreEqual(_mode.Id, _dut.ModeId);
            Assert.AreEqual(_mode.ForSupplier, _dut.IsSupplierStep);
            Assert.AreEqual(_responsible.Id, _dut.ResponsibleId);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenTitleNotGiven() =>
            Assert.ThrowsException<ArgumentNullException>(() =>
                new Step(TestPlant, null, _mode, _responsible)
            );

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenModeNotGiven() =>
            Assert.ThrowsException<ArgumentNullException>(() =>
                new Step(TestPlant, "S", null, _responsible)
            );

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenResponsibleNotGiven() =>
            Assert.ThrowsException<ArgumentNullException>(() =>
                new Step(TestPlant, "S", _mode, null)
            );

        [TestMethod]
        public void SetMode_ShouldSetMode()
        {
            var modeId = 1;
            var mode = new Mode(_dut.Plant, "ModeTitle", false);
            mode.SetProtectedIdForTesting(modeId);
            _dut.SetMode(mode);

            Assert.AreEqual(modeId, _dut.ModeId);
            Assert.AreEqual(mode.ForSupplier, _dut.IsSupplierStep);
        }
        
        [TestMethod]
        public void SetMode_ShouldAddModifiedEvent()
        {
            var modeId = 1;
            var mode = new Mode(_dut.Plant, "ModeTitle", false);
            mode.SetProtectedIdForTesting(modeId);
            _dut.SetMode(mode);
            
            var eventTypes = _dut.DomainEvents.Select(e => e.GetType()).ToList();
            CollectionAssert.Contains(eventTypes, typeof(ModifiedEvent<Step>));
        }

        [TestMethod]
        public void SetResponsible_ShouldSetResponsible()
        {
            var responsibleId = 1;
            var responsible = new Responsible(_dut.Plant, "C", "Desc");
            responsible.SetProtectedIdForTesting(responsibleId);
            _dut.SetResponsible(responsible);

            Assert.AreEqual(responsibleId, _dut.ResponsibleId);
        }
        
        [TestMethod]
        public void SetResponsible_ShouldAddModifiedEvent()
        {
            var responsibleId = 1;
            var responsible = new Responsible(_dut.Plant, "C", "Desc");
            responsible.SetProtectedIdForTesting(responsibleId);
            _dut.SetResponsible(responsible);
            
            var eventTypes = _dut.DomainEvents.Select(e => e.GetType()).ToList();
            CollectionAssert.Contains(eventTypes, typeof(ModifiedEvent<Step>));
        }
    }
}
