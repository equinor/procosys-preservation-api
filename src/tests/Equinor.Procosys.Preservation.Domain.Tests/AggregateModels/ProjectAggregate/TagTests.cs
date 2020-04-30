using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Action = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class TagTests
    {
        #region Setup

        private const string TestPlant = "PlantA";

        private const int TwoWeeksInterval = 2;
        private const int ThreeWeeksInterval = 3;
        private Person _person;
        private Mock<Step> _step1Mock;
        private Mock<Step> _step2Mock;
        
        private RequirementDefinition _reqDef1NotNeedInput;
        private RequirementDefinition _reqDef2NotNeedInput;
        private RequirementDefinition _reqDef1NeedInput;
        private RequirementDefinition _reqDef2NeedInput;
        private TagRequirement _reqNotNeedInputTwoWeekInterval;
        private TagRequirement _reqNotNeedInputThreeWeekInterval;
        private TagRequirement _reqNeedInputTwoWeekInterval;
        private TagRequirement _reqNeedInputThreeWeekInterval;
        private List<TagRequirement> _twoReqs_NoneNeedInput_DifferentIntervals;
        private List<TagRequirement> _oneReq_NotNeedInputTwoWeekInterval;
        private List<TagRequirement> _oneReq_NeedInputTwoWeekInterval;
        private List<TagRequirement> _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval;
        private List<TagRequirement> _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval;
        private List<TagRequirement> _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval;

        private ManualTimeProvider _timeProvider;
        private DateTime _utcNow;
        private Tag _dutWithOneReqNotNeedInputTwoWeekInterval;
        private Journey _journey;

        [TestInitialize]
        public void Setup()
        {
            _step1Mock = new Mock<Step>();
            _step1Mock.SetupGet(x => x.Id).Returns(3);
            _step1Mock.SetupGet(x => x.Plant).Returns(TestPlant);
            _step1Mock.Object.SortKey = 10;
            _step2Mock = new Mock<Step>();
            _step2Mock.SetupGet(x => x.Id).Returns(4);
            _step2Mock.SetupGet(x => x.Plant).Returns(TestPlant);
            _step2Mock.Object.SortKey = 20;

            _person = new Mock<Person>().Object;

            _journey = new Journey(TestPlant, "J");
            _journey.AddStep(_step1Mock.Object);
            _journey.AddStep(_step2Mock.Object);

            var reqDefId = 1;

            var reqDef1NotNeedInputMock = new Mock<RequirementDefinition>(TestPlant, "", 2, 0);
            reqDef1NotNeedInputMock.SetupGet(r => r.Id).Returns(reqDefId++);
            reqDef1NotNeedInputMock.SetupGet(r => r.Plant).Returns(TestPlant);
            _reqDef1NotNeedInput = reqDef1NotNeedInputMock.Object;
            _reqDef1NotNeedInput.AddField(new Field(TestPlant, "", FieldType.Info, 0));

            var reqDef2NotNeedInputMock = new Mock<RequirementDefinition>(TestPlant, "", 2, 0);
            reqDef2NotNeedInputMock.SetupGet(r => r.Id).Returns(reqDefId++);
            reqDef2NotNeedInputMock.SetupGet(r => r.Plant).Returns(TestPlant);
            _reqDef2NotNeedInput = reqDef2NotNeedInputMock.Object;
            _reqDef2NotNeedInput.AddField(new Field(TestPlant, "", FieldType.Info, 0));
            
            var reqDef1NeedInputMock = new Mock<RequirementDefinition>(TestPlant, "", 1, 0);
            reqDef1NeedInputMock.SetupGet(r => r.Id).Returns(reqDefId++);
            reqDef1NeedInputMock.SetupGet(r => r.Plant).Returns(TestPlant);
            _reqDef1NeedInput = reqDef1NeedInputMock.Object;
            _reqDef1NeedInput.AddField(new Field(TestPlant, "", FieldType.CheckBox, 0));
            
            var reqDef2NeedInputMock = new Mock<RequirementDefinition>(TestPlant, "", 1, 0);
            reqDef2NeedInputMock.SetupGet(r => r.Id).Returns(reqDefId);
            reqDef2NeedInputMock.SetupGet(r => r.Plant).Returns(TestPlant);
            _reqDef2NeedInput = reqDef2NeedInputMock.Object;
            _reqDef2NeedInput.AddField(new Field(TestPlant, "", FieldType.CheckBox, 0));
            
            _reqNotNeedInputTwoWeekInterval = new TagRequirement(TestPlant, TwoWeeksInterval, _reqDef1NotNeedInput);
            _reqNotNeedInputThreeWeekInterval = new TagRequirement(TestPlant, ThreeWeeksInterval, _reqDef2NotNeedInput);
            _reqNeedInputTwoWeekInterval = new TagRequirement(TestPlant, TwoWeeksInterval, _reqDef1NeedInput);
            _reqNeedInputThreeWeekInterval = new TagRequirement(TestPlant, ThreeWeeksInterval, _reqDef2NeedInput);

            _twoReqs_NoneNeedInput_DifferentIntervals = new List<TagRequirement>
            {
                _reqNotNeedInputTwoWeekInterval, _reqNotNeedInputThreeWeekInterval
            };

            _oneReq_NotNeedInputTwoWeekInterval = new List<TagRequirement>
            {
                _reqNotNeedInputTwoWeekInterval
            };

            _oneReq_NeedInputTwoWeekInterval = new List<TagRequirement>
            {
                _reqNeedInputTwoWeekInterval
            };

            _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval = new List<TagRequirement>
            {
                _reqNeedInputTwoWeekInterval, _reqNotNeedInputThreeWeekInterval
            };

            _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval = new List<TagRequirement>
            {
                _reqNotNeedInputTwoWeekInterval, _reqNeedInputThreeWeekInterval
            };

            _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval = new List<TagRequirement>
            {
                _reqNotNeedInputTwoWeekInterval, _reqNotNeedInputThreeWeekInterval
            };

            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            _timeProvider = new ManualTimeProvider(_utcNow);
            TimeService.SetProvider(_timeProvider);

            _dutWithOneReqNotNeedInputTwoWeekInterval = new Tag(
                TestPlant,
                TagType.Standard,
                "TagNoA",
                "DescA", 
                _step1Mock.Object,
                _oneReq_NotNeedInputTwoWeekInterval);
        }
        #endregion

        #region Constructor

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            Assert.AreEqual(TestPlant, _dutWithOneReqNotNeedInputTwoWeekInterval.Plant);
            Assert.AreEqual("TagNoA", _dutWithOneReqNotNeedInputTwoWeekInterval.TagNo);
            Assert.AreEqual("DescA", _dutWithOneReqNotNeedInputTwoWeekInterval.Description);
            Assert.AreEqual(_step1Mock.Object.Id, _dutWithOneReqNotNeedInputTwoWeekInterval.StepId);
            Assert.AreEqual(TagType.Standard, _dutWithOneReqNotNeedInputTwoWeekInterval.TagType);
            Assert.IsNull(_dutWithOneReqNotNeedInputTwoWeekInterval.NextDueTimeUtc);
            var requirements = _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements;
            Assert.AreEqual(1, requirements.Count);
            
            var req = _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.ElementAt(0);
            Assert.IsNull(req.NextDueTimeUtc);
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);
        }
        
        [TestMethod]
        public void Constructor_ShouldNotSetReadyToBePreserved_AtAnyTime()
        {
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBePreserved());
            
            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBePreserved());

            _timeProvider.SetTime(_utcNow);
            _timeProvider.ElapseWeeks(ThreeWeeksInterval);
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBePreserved());
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenStepNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(()
                => new Tag(TestPlant, TagType.Standard, "", "", null, _twoReqs_NoneNeedInput_DifferentIntervals));

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenRequirementsNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(()
                => new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, null));

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenEmptyListOfRequirementsGiven()
            => Assert.ThrowsException<Exception>(()
                => new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, new List<TagRequirement>()));

        #endregion

        #region SetStep

        [TestMethod]
        public void SetStep_ShouldSetStepId()
        {
            var newStep = new Mock<Step>();
            newStep.SetupGet(x => x.Id).Returns(4);
            _dutWithOneReqNotNeedInputTwoWeekInterval.SetStep(newStep.Object);

            Assert.AreEqual(newStep.Object.Id, _dutWithOneReqNotNeedInputTwoWeekInterval.StepId);
        }

        [TestMethod]
        public void SetStep_ShouldThrowException_WhenStepNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dutWithOneReqNotNeedInputTwoWeekInterval.SetStep(null));

        #endregion

        #region AddRequirement

        [TestMethod]
        public void AddRequirement_ShouldAddRequirement()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.AddRequirement(_reqNeedInputThreeWeekInterval);

            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.Count);
            Assert.AreEqual(_reqNeedInputThreeWeekInterval, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.Last());
        }

        [TestMethod]
        public void AddRequirement_ShouldThrowException_WhenRequirementNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dutWithOneReqNotNeedInputTwoWeekInterval.AddRequirement(null));

        [TestMethod]
        public void AddRequirement_ShouldThrowException_WhenRequirementWithSameDefinitionExists()
            => Assert.ThrowsException<ArgumentException>(() =>
                _dutWithOneReqNotNeedInputTwoWeekInterval.AddRequirement(_dutWithOneReqNotNeedInputTwoWeekInterval
                    .Requirements.First()));

        #endregion

        #region StartPreservation

        [TestMethod]
        public void StartPreservation_ShouldSetStatusActive()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);

            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();

            Assert.AreEqual(PreservationStatus.Active, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);
        }

        [TestMethod]
        public void StartPreservation_ShouldSetCorrectNextDueDateOnTagAndEachRequirement()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _twoReqs_NoneNeedInput_DifferentIntervals);

            dut.StartPreservation();

            var expectedNextDueTimeFirstUtc = _utcNow.AddWeeks(TwoWeeksInterval);
            var expectedNextDueTimeLaterUtc = _utcNow.AddWeeks(ThreeWeeksInterval);
            Assert.AreEqual(expectedNextDueTimeFirstUtc, dut.Requirements.ElementAt(0).NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeLaterUtc, dut.Requirements.ElementAt(1).NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeFirstUtc, dut.NextDueTimeUtc);
        }

        [TestMethod]
        public void StartPreservation_ShouldStartOnEachNonVoidedRequirement()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _twoReqs_NoneNeedInput_DifferentIntervals);
            dut.Requirements.ElementAt(0).Void();

            dut.StartPreservation();

            var expectedNextDueTime = _utcNow.AddWeeks(dut.Requirements.ElementAt(1).IntervalWeeks);
            Assert.AreEqual(expectedNextDueTime, dut.Requirements.ElementAt(1).NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTime, dut.NextDueTimeUtc);
            Assert.IsFalse(dut.Requirements.ElementAt(0).NextDueTimeUtc.HasValue);
        }

        [TestMethod]
        public void StartPreservation_ShouldThrowException_IfAlreadyStarted()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _twoReqs_NoneNeedInput_DifferentIntervals);

            dut.StartPreservation();

            Assert.ThrowsException<Exception>(() => dut.StartPreservation());
        }

        #endregion

        #region IsReadyToBePreserved

        [TestMethod]
        public void IsReadyToBePreserved_ShouldBeFalse_BeforePeriod()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();

            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBePreserved());
        }
        
        [TestMethod]
        public void IsReadyToBePreserved_ShouldBeTrue_InPeriod_WhenNotNeedInput()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBePreserved());
        }
        
        [TestMethod]
        public void IsReadyToBePreserved_ShouldBeTrue_OnOverdue_WhenNotNeedInput()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();

            _timeProvider.ElapseWeeks(TwoWeeksInterval + TwoWeeksInterval);
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBePreserved());
        }

        [TestMethod]
        public void IsReadyToBePreserved_ShouldBeFalse_InPeriod_WhenNeedInput()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _oneReq_NeedInputTwoWeekInterval);
            Assert.AreEqual(PreservationStatus.NotStarted, dut.Status);

            dut.StartPreservation();

            _timeProvider.ElapseWeeks(TwoWeeksInterval + TwoWeeksInterval);
            Assert.IsFalse(dut.IsReadyToBePreserved());
        }

        #endregion

        #region Preserve

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenPreservingBeforeTime()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();
                   
            Assert.ThrowsException<Exception>(() 
                => _dutWithOneReqNotNeedInputTwoWeekInterval.Preserve(_person));
        }

        [TestMethod]
        public void Preserve_ShouldPreserve_WhenPreservingOnDue()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();
            Assert.AreEqual(1, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            _dutWithOneReqNotNeedInputTwoWeekInterval.Preserve(_person);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);
        }

        [TestMethod]
        public void Preserve_ShouldPreserve_WhenPreservingOverDue()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();
            Assert.AreEqual(1, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);

            _timeProvider.ElapseWeeks(ThreeWeeksInterval);
            _dutWithOneReqNotNeedInputTwoWeekInterval.Preserve(_person);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);
        }
        
        [TestMethod]
        public void Preserve_ShouldPreserveOnce_WhenPreservingMultipleTimesAtSameTime()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();
            Assert.AreEqual(1, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            _dutWithOneReqNotNeedInputTwoWeekInterval.BulkPreserve(_person);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);

            Assert.ThrowsException<Exception>(() 
                => _dutWithOneReqNotNeedInputTwoWeekInterval.Preserve(_person));
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenRequirementNeedsInput()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _oneReq_NeedInputTwoWeekInterval);
            dut.StartPreservation();

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            Assert.ThrowsException<Exception>(() => dut.Preserve(_person));
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenFirstUpcomingRequirementNeedsInput()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation();

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            Assert.ThrowsException<Exception>(() => dut.Preserve(_person));
        }
        
        [TestMethod]
        public void Preserve_ShouldThrowException_WhenFirstUpcomingRequirementNeedInput_BecauseFirstInListIsVoided()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);
            dut.StartPreservation();
            dut.Requirements.ElementAt(0).Void();

            _timeProvider.ElapseWeeks(ThreeWeeksInterval);
            Assert.ThrowsException<Exception>(() => dut.Preserve(_person));
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenPreservedByNotGiven()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _twoReqs_NoneNeedInput_DifferentIntervals);
            dut.StartPreservation();

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            Assert.ThrowsException<ArgumentNullException>(() => dut.Preserve(null));
        }
        
        [TestMethod]
        public void Preserve_ShouldPreserveDueRequirementsOnly()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);
            dut.StartPreservation();

            var req1 = dut.Requirements.ElementAt(0);
            var req2 = dut.Requirements.ElementAt(1);
            Assert.AreEqual(1, req1.PreservationPeriods.Count);
            Assert.AreEqual(1, req2.PreservationPeriods.Count);

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            dut.Preserve(_person);
            Assert.AreEqual(2, req1.PreservationPeriods.Count);
            Assert.AreEqual(1, req2.PreservationPeriods.Count);
        }
        
        [TestMethod]
        public void Preserve_ShouldChange_NextDueTime()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation();
            
            var requirement = dut.OrderedRequirements().First();
            var oldNextTime = requirement.NextDueTimeUtc;
            Assert.AreEqual(_reqNotNeedInputTwoWeekInterval, requirement);
            Assert.AreEqual(requirement.NextDueTimeUtc, dut.NextDueTimeUtc);

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            dut.Preserve(_person);

            requirement = dut.OrderedRequirements().First();
            Assert.AreEqual(_reqNotNeedInputThreeWeekInterval, requirement);
            Assert.AreEqual(requirement.NextDueTimeUtc, dut.NextDueTimeUtc);
            Assert.AreNotEqual(oldNextTime, requirement.NextDueTimeUtc);
        }

        #endregion

        #region Preserve Requirement
        
        [TestMethod]
        public void PreserveRequirement_ShouldThrowException_WhenRequirementNeedsInput()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _oneReq_NeedInputTwoWeekInterval);
            dut.StartPreservation();
            _timeProvider.ElapseWeeks(TwoWeeksInterval);

            Assert.ThrowsException<Exception>(() => dut.Preserve(_person, dut.Requirements.Single().Id));
        }

        [TestMethod]
        public void PreserveRequirement_ShouldThrowException_WhenPreservedByNotGiven()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();
            _timeProvider.ElapseWeeks(TwoWeeksInterval);

            Assert.ThrowsException<ArgumentNullException>(() => dut.Preserve(null, dut.Requirements.Single().Id));
        }
        
        [TestMethod]
        public void PreserveRequirement_ShouldPreserve()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            var requirement = dut.Requirements.Single();
            Assert.AreEqual(1, requirement.PreservationPeriods.Count);

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            dut.Preserve(_person, dut.Requirements.Single().Id);
            Assert.AreEqual(2, requirement.PreservationPeriods.Count);
        }

        [TestMethod]
        public void PreserveRequirement_ShouldChange_NextDueTime()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();
            
            var requirement = dut.Requirements.Single();
            var oldNextTime = requirement.NextDueTimeUtc;
            Assert.AreEqual(requirement.NextDueTimeUtc, dut.NextDueTimeUtc);
            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            dut.Preserve(_person, requirement.Id);

            Assert.AreEqual(requirement.NextDueTimeUtc, dut.NextDueTimeUtc);
            Assert.AreNotEqual(oldNextTime, requirement.NextDueTimeUtc);
        }
        #endregion

        #region BulkPreserve

        [TestMethod]
        public void BulkPreserve_ShouldThrowException_WhenRequirementNeedInput()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _oneReq_NeedInputTwoWeekInterval);
            dut.StartPreservation();

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            Assert.ThrowsException<Exception>(() => dut.BulkPreserve(_person));
        }

        [TestMethod]
        public void BulkPreserve_ShouldThrowException_WhenFirstUpcomingRequirementNeedInput()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation();

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            Assert.ThrowsException<Exception>(() => dut.BulkPreserve(_person));
        }

        [TestMethod]
        public void BulkPreserve_ShouldThrowException_WhenFirstUpcomingRequirementNeedInput_BecauseFirstInListIsVoided()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);
            dut.StartPreservation();
            dut.Requirements.ElementAt(0).Void();

            _timeProvider.ElapseWeeks(ThreeWeeksInterval);
            Assert.ThrowsException<Exception>(() => dut.BulkPreserve(_person));
        }

        [TestMethod]
        public void BulkPreserve_ShouldThrowException_WhenPreservedByNotGiven()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _twoReqs_NoneNeedInput_DifferentIntervals);
            dut.StartPreservation();

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            Assert.ThrowsException<ArgumentNullException>(() => dut.BulkPreserve(null));
        }

        [TestMethod]
        public void BulkPreserve_ShouldThrowException_WhenPreservingBeforeTime()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();
            
            Assert.ThrowsException<Exception>(() 
                => _dutWithOneReqNotNeedInputTwoWeekInterval.BulkPreserve(_person));
        }

        [TestMethod]
        public void BulkPreserve_ShouldPreserve_WhenPreservingOnDue()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();
            Assert.AreEqual(1, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            _dutWithOneReqNotNeedInputTwoWeekInterval.BulkPreserve(_person);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);
        }

        [TestMethod]
        public void BulkPreserve_ShouldPreserve_WhenPreservingOverDue()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();
            Assert.AreEqual(1, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);

            _timeProvider.ElapseWeeks(ThreeWeeksInterval);
            _dutWithOneReqNotNeedInputTwoWeekInterval.BulkPreserve(_person);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);
        }
        
        [TestMethod]
        public void BulkPreserve_ShouldPreserveOnce_WhenPreservingMultipleTimesAtSameTime()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();
            Assert.AreEqual(1, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            _dutWithOneReqNotNeedInputTwoWeekInterval.BulkPreserve(_person);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);

            Assert.ThrowsException<Exception>(() 
                => _dutWithOneReqNotNeedInputTwoWeekInterval.BulkPreserve(_person));
        }
        
        [TestMethod]
        public void BulkPreserve_ShouldPreserveDueRequirementsOnly()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);
            dut.StartPreservation();

            var req1 = dut.Requirements.ElementAt(0);
            var req2 = dut.Requirements.ElementAt(1);
            Assert.AreEqual(1, req1.PreservationPeriods.Count);
            Assert.AreEqual(1, req2.PreservationPeriods.Count);

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            dut.BulkPreserve(_person);
            Assert.AreEqual(2, req1.PreservationPeriods.Count);
            Assert.AreEqual(1, req2.PreservationPeriods.Count);
        }

        [TestMethod]
        public void BulkPreserve_ShouldChange_NextDueTime()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation();
            
            var requirement = dut.OrderedRequirements().First();
            var oldNextTime = requirement.NextDueTimeUtc;
            Assert.AreEqual(_reqNotNeedInputTwoWeekInterval, requirement);
            Assert.AreEqual(requirement.NextDueTimeUtc, dut.NextDueTimeUtc);
            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            dut.BulkPreserve(_person);

            requirement = dut.OrderedRequirements().First();
            Assert.AreEqual(_reqNotNeedInputThreeWeekInterval, requirement);
            Assert.AreEqual(requirement.NextDueTimeUtc, dut.NextDueTimeUtc);
            Assert.AreNotEqual(oldNextTime, requirement.NextDueTimeUtc);
        }
        
        #endregion

        #region GetUpComingRequirements

        [TestMethod]
        public void GetUpComingRequirements_ShouldReturnNoneRequirements_BeforePreservationStarted()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);

            _timeProvider.ElapseWeeks(ThreeWeeksInterval);
            Assert.AreEqual(0, dut.GetUpComingRequirements().Count());
        }
        
        [TestMethod]
        public void GetUpComingRequirements_ShouldReturnNoneRequirements_BeforeDue()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);

            dut.StartPreservation();
            Assert.AreEqual(0, dut.GetUpComingRequirements().Count());
        }
        
        [TestMethod]
        public void GetUpComingRequirements_ShouldReturnTwoReadyRequirements_WhenBothReadyAndDue()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation();

            _timeProvider.ElapseWeeks(ThreeWeeksInterval);
            Assert.AreEqual(2, dut.GetUpComingRequirements().Count());
        }
        
        [TestMethod]
        public void GetUpComingRequirements_ShouldReturnReadyRequirements_WhenOverDue()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation();

            _timeProvider.ElapseWeeks(ThreeWeeksInterval);
            Assert.AreEqual(1, dut.GetUpComingRequirements().Count());
        }
        
        [TestMethod]
        public void GetUpComingRequirements_ShouldReturnReadyRequirements_WhenDue()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);
            dut.StartPreservation();

            _timeProvider.ElapseWeeks(ThreeWeeksInterval);
            Assert.AreEqual(1, dut.GetUpComingRequirements().Count());
        }
        
        [TestMethod]
        public void GetUpComingRequirements_ShouldNotReturnVoidedRequirements()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation();
            dut.Requirements.ElementAt(0).Void();

            _timeProvider.ElapseWeeks(ThreeWeeksInterval);
            Assert.AreEqual(1, dut.GetUpComingRequirements().Count());
        }
        
        #endregion

        #region OrderedRequirements

        [TestMethod]
        public void OrderedRequirements_ShouldReturnAllRequirements_BeforeAndAfterPreservationStarted()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);

            Assert.AreEqual(2, dut.OrderedRequirements().Count());
        
            dut.StartPreservation();
            
            Assert.AreEqual(2, dut.OrderedRequirements().Count());
        }
        
        [TestMethod]
        public void OrderedRequirements_ShouldNotReturnVoidedRequirements()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);
            dut.StartPreservation();
            dut.Requirements.ElementAt(0).Void();

            Assert.AreEqual(1, dut.OrderedRequirements().Count());
        }

        [TestMethod]
        public void OrderedRequirements_ShouldChange_WhenPreserve()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation();
            Assert.AreEqual(_reqNotNeedInputTwoWeekInterval, dut.OrderedRequirements().First());

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            dut.Preserve(_person);

            Assert.AreEqual(_reqNotNeedInputThreeWeekInterval, dut.OrderedRequirements().First());
        }

        [TestMethod]
        public void OrderedRequirements_ShouldChange_WhenBulkPreserve()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation();
            Assert.AreEqual(_reqNotNeedInputTwoWeekInterval, dut.OrderedRequirements().First());

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            dut.BulkPreserve(_person);

            Assert.AreEqual(_reqNotNeedInputThreeWeekInterval, dut.OrderedRequirements().First());
        }

        #endregion

        #region Transfer

        [TestMethod]
        public void Transfer_ShouldTransferToNextStep()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();
            dut.Transfer(_journey);

            Assert.AreEqual(_step2Mock.Object.Id, dut.StepId);
        }
                
        [TestMethod]
        public void Transfer_ShouldThrowException_WhenJourneyIsNull()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            Assert.ThrowsException<ArgumentNullException>(() => dut.Transfer(null));
        }
                
        [TestMethod]
        public void Transfer_ShouldThrowException_WhenTagIsSiteArea()
        {
            var dut = new Tag(TestPlant, TagType.SiteArea, "", "", _step1Mock.Object, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            Assert.ThrowsException<Exception>(() => dut.Transfer(_journey));
        }

        #endregion

        #region StopPreservation

        [TestMethod]
        public void StopPreservation_ShouldSetStatusToCompleted_WhenInLastStepAndIsStandard()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step2Mock.Object, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            dut.StopPreservation(_journey);

            Assert.AreEqual(PreservationStatus.Completed, dut.Status);
        }

        [TestMethod]
        public void StopPreservation_ShouldThrowException_WhenJourneyIsNull()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            Assert.ThrowsException<ArgumentNullException>(() => dut.StopPreservation(null));
        }

        [TestMethod]
        public void StopPreservation_ShouldSetStatusToCompleted_WhenNotInLastStepAndIsArea()
        {
            var dut = new Tag(TestPlant, TagType.SiteArea, "", "", _step1Mock.Object, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            dut.StopPreservation(_journey);

            Assert.AreEqual(PreservationStatus.Completed, dut.Status);
        }


        #endregion

        #region IsReadyToBeTransferred

        [TestMethod]
        public void IsReadyToBeTransferred_ShouldBeFalse_BeforePreservationStarted()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _oneReq_NotNeedInputTwoWeekInterval);

            Assert.IsFalse(dut.IsReadyToBeTransferred(_journey));
        }
        
        [TestMethod]
        public void IsReadyToBeTransferred_ShouldBeTrue_AfterPreservationStarted()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            Assert.IsTrue(dut.IsReadyToBeTransferred(_journey));
        }
        
        [TestMethod]
        public void IsReadyToBeTransferred_ShouldBeFalse_WhenCurrentStepIsLastStepInJourney()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step2Mock.Object, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            Assert.IsFalse(dut.IsReadyToBeTransferred(_journey));
        }
                
        [TestMethod]
        public void IsReadyToBeTransferred_ShouldThrowException_WhenJourneyIsNull()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            Assert.ThrowsException<ArgumentNullException>(() => dut.IsReadyToBeTransferred(null));
        }

        #endregion
        
        #region IsReadyToBeStopped

        [TestMethod]
        public void IsReadyToBeStopped_ShouldBeFalse_BeforePreservationStarted()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step2Mock.Object, _oneReq_NotNeedInputTwoWeekInterval);

            Assert.IsFalse(dut.IsReadyToBeStopped(_journey));
        }

        [TestMethod]
        public void IsReadyToBeStopped_ShouldBeTrue_AfterPreservationStarted()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step2Mock.Object, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            Assert.IsTrue(dut.IsReadyToBeStopped(_journey));
        }

        [TestMethod]
        public void IsReadyToBeStopped_ShouldBeFalse_WhenCurrentStepIsLastStepInJourney()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            Assert.IsFalse(dut.IsReadyToBeStopped(_journey));
        }

        [TestMethod]
        public void IsReadyToBeStopped_ShouldThrowException_WhenJourneyIsNull()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            Assert.ThrowsException<ArgumentNullException>(() => dut.IsReadyToBeStopped(null));
        }

        #endregion

        #region IsReadyToBeStarted

        [TestMethod]
        public void IsReadyToBeStarted_ShouldBeTrue_BeforePreservationStarted()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _oneReq_NotNeedInputTwoWeekInterval);

            Assert.IsTrue(dut.IsReadyToBeStarted());
        }
        
        [TestMethod]
        public void IsReadyToBeStarted_ShouldBeFalse_AfterPreservationStarted()
        {
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            Assert.IsFalse(dut.IsReadyToBeStarted());
        }

        #endregion
        
        #region AddAction

        [TestMethod]
        public void AddAction_ShouldAddAction()
        {
            var action = new Action(TestPlant, "", "", null);
            _dutWithOneReqNotNeedInputTwoWeekInterval.AddAction(action);

            Assert.AreEqual(action, _dutWithOneReqNotNeedInputTwoWeekInterval.Actions.First());
        }

        [TestMethod]
        public void AddAction_ShouldThrowException_WhenActionNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dutWithOneReqNotNeedInputTwoWeekInterval.AddAction(null));

        #endregion

        #region SetArea

        [TestMethod]
        public void SetArea_ShouldSetAreaProperties()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.SetArea("AC", "AD");

            Assert.AreEqual("AC", _dutWithOneReqNotNeedInputTwoWeekInterval.AreaCode);
            Assert.AreEqual("AD", _dutWithOneReqNotNeedInputTwoWeekInterval.AreaDescription);
        }

        #endregion

        #region SetDiscipline

        [TestMethod]
        public void SetDiscipline_ShouldSetDisciplineProperties()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.SetDiscipline("DC", "DD");

            Assert.AreEqual("DC", _dutWithOneReqNotNeedInputTwoWeekInterval.DisciplineCode);
            Assert.AreEqual("DD", _dutWithOneReqNotNeedInputTwoWeekInterval.DisciplineDescription);
        }

        #endregion
        
        #region AddAttachment

        [TestMethod]
        public void AddAttachment_ShouldAddAttachment()
        {
            var attachment = new TagAttachment(TestPlant, Guid.Empty, null, null);
            _dutWithOneReqNotNeedInputTwoWeekInterval.AddAttachment(attachment);

            Assert.AreEqual(attachment, _dutWithOneReqNotNeedInputTwoWeekInterval.Attachments.First());
        }

        [TestMethod]
        public void AddAttachment_ShouldThrowException_WhenAttachmentNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dutWithOneReqNotNeedInputTwoWeekInterval.AddAttachment(null));

        #endregion
        
        #region GetAttachmentByFileName

        [TestMethod]
        public void GetAttachmentByFileName_ShouldGetAttachmentWhenExists()
        {
            // Arrange
            var fileName = "FileA";
            var attachment = new TagAttachment(TestPlant, Guid.Empty, null, fileName);
            _dutWithOneReqNotNeedInputTwoWeekInterval.AddAttachment(attachment);

            // Act
            var result = _dutWithOneReqNotNeedInputTwoWeekInterval.GetAttachmentByFileName(fileName);

            // Arrange
            Assert.AreEqual(attachment, result);
        }

        [TestMethod]
        public void GetAttachmentByFileName_ShouldGetAttachmentWhenExists_RegardlessOfCasing()
        {
            // Arrange
            var fileName = "FileA";
            var attachment = new TagAttachment(TestPlant, Guid.Empty, null, fileName);
            _dutWithOneReqNotNeedInputTwoWeekInterval.AddAttachment(attachment);

            // Act
            var result = _dutWithOneReqNotNeedInputTwoWeekInterval.GetAttachmentByFileName(fileName.ToUpper());

            // Arrange
            Assert.AreEqual(attachment, result);
        }

        [TestMethod]
        public void GetAttachmentByFileName_ShouldReturnNullWhenNotFound()
        {
            // Act
            var result = _dutWithOneReqNotNeedInputTwoWeekInterval.GetAttachmentByFileName("FileA");

            // Arrange
            Assert.IsNull(result);
        }

        #endregion
        
        #region FollowsAJourney

        [TestMethod]
        public void FollowsAJourney_ShouldGetBeTrueForStandardTag()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.Standard, "", "", _step1Mock.Object, _oneReq_NotNeedInputTwoWeekInterval);

            // Act and Arrange
            Assert.IsTrue(dut.FollowsAJourney);
        }

        [TestMethod]
        public void FollowsAJourney_ShouldGetBeFalseForSiteAreaTag()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.SiteArea, "", "", _step1Mock.Object, _oneReq_NotNeedInputTwoWeekInterval);

            // Act and Arrange
            Assert.IsFalse(dut.FollowsAJourney);
        }

        [TestMethod]
        public void FollowsAJourney_ShouldGetBeTrueForPreAreaTag()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.PreArea, "", "", _step1Mock.Object, _oneReq_NotNeedInputTwoWeekInterval);

            // Act and Arrange
            Assert.IsTrue(dut.FollowsAJourney);
        }

        [TestMethod]
        public void FollowsAJourney_ShouldGetBeFalseForPoAreaTag()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.PoArea, "", "", _step1Mock.Object, _oneReq_NotNeedInputTwoWeekInterval);

            // Act and Arrange
            Assert.IsFalse(dut.FollowsAJourney);
        }

        #endregion
    }
}
