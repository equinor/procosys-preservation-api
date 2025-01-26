using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagCommands.DuplicateAreaTag;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Area;
using Equinor.ProCoSys.Preservation.MainApi.Discipline;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.DuplicateAreaTag
{
    [TestClass]
    public class DuplicateAreaTagCommandHandlerTests : CommandHandlerTestsBase
    {
        private DuplicateAreaTagCommand _command;
        private Tag _sourceTag;
        private TagRequirement _req1;
        private TagRequirement _req2;
        private Mock<RequirementDefinition> _rd1Mock;
        private Mock<RequirementDefinition> _rd2Mock;

        private int _stepId = 11;
        private int _rdId1 = 17;
        private int _rdId2 = 18;
        private int _sourceTagId = 7;

        private DuplicateAreaTagCommandHandler _dut;
        private Project _project;

        private readonly int _interval1 = 2;
        private readonly int _interval2 = 3;
        private readonly string _disciplineDescription = "DisciplineDescription";
        private readonly string _areaDescription = "AreaDescription";

        [TestInitialize]
        public void Setup()
        {
            var stepMock = new Mock<Step>();
            stepMock.SetupGet(s => s.Plant).Returns(TestPlant);
            stepMock.SetupGet(s => s.Id).Returns(_stepId);
                        
            var journeyRepositoryMock = new Mock<IJourneyRepository>();
            journeyRepositoryMock
                .Setup(x => x.GetStepByStepIdAsync(_stepId))
                .Returns(Task.FromResult(stepMock.Object));

            _rd1Mock = new Mock<RequirementDefinition>();
            _rd1Mock.SetupGet(rd => rd.Id).Returns(_rdId1);
            _rd1Mock.SetupGet(rd => rd.Plant).Returns(TestPlant);
            _rd2Mock = new Mock<RequirementDefinition>();
            _rd2Mock.SetupGet(rd => rd.Id).Returns(_rdId2);
            _rd2Mock.SetupGet(rd => rd.Plant).Returns(TestPlant);
            
            var rtRepositoryMock = new Mock<IRequirementTypeRepository>();
            rtRepositoryMock
                .Setup(r => r.GetRequirementDefinitionsByIdsAsync(new List<int> {_rdId1, _rdId2}))
                .Returns(Task.FromResult(new List<RequirementDefinition> {_rd1Mock.Object, _rd2Mock.Object}));

            _req1 = new TagRequirement(TestPlant, _interval1, _rd1Mock.Object);
            _req2 = new TagRequirement(TestPlant, _interval2, _rd2Mock.Object);
            
            _project = new Project(TestPlant, "TestProjectName", "" ,ProjectProCoSysGuid );
            _sourceTag = new Tag(
                TestPlant,
                TagType.SiteArea,
                Guid.NewGuid(),
                "TagNo",
                "Desc",
                stepMock.Object,
                new List<TagRequirement> {_req1, _req2});
            _sourceTag.SetArea("AC", "AD");
            _sourceTag.SetDiscipline("DC", "DD");

            _sourceTag.SetProtectedIdForTesting(_sourceTagId);
            _project.AddTag(_sourceTag);

            var projectRepoMock = new Mock<IProjectRepository>();
            projectRepoMock.Setup(r => r.GetTagWithPreservationHistoryByTagIdAsync(_sourceTagId)).Returns(Task.FromResult(_sourceTag));
            projectRepoMock
                .Setup(r => r.GetProjectOnlyByTagIdAsync(_sourceTagId)).Returns(Task.FromResult(_project));
            
            var disciplineCode = "D";
            var disciplineApiServiceMock = new Mock<IDisciplineApiService>();
            disciplineApiServiceMock.Setup(s => s.TryGetDisciplineAsync(TestPlant, disciplineCode, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new PCSDiscipline
                {
                    Code = disciplineCode,
                    Description = _disciplineDescription
                }));

            var areaCode = "A";
            var areaApiServiceMock = new Mock<IAreaApiService>();
            areaApiServiceMock.Setup(s => s.TryGetAreaAsync(TestPlant, areaCode, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new PCSArea
                {
                    Code = areaCode,
                    Description = _areaDescription
                }));

            _command = new DuplicateAreaTagCommand(_sourceTagId,TagType.SiteArea, disciplineCode, areaCode, "-01", "Desc", "Rem", "SA");

            _dut = new DuplicateAreaTagCommandHandler(
                projectRepoMock.Object,
                journeyRepositoryMock.Object,
                rtRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object,
                disciplineApiServiceMock.Object,
                areaApiServiceMock.Object);
        }
        
        [TestMethod]
        public async Task HandlingDuplicateAreaTagCommand_ShouldAddTagToExistingProject()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            var duplicatedTag = _project.Tags.SingleOrDefault(t => t.TagNo == _command.GetTagNo());
            Assert.IsNotNull(duplicatedTag);
        }

        [TestMethod]
        public async Task HandlingDuplicateAreaTagCommand_ShouldSetCorrectProperties()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            var duplicatedTag = _project.Tags.Single(t => t.TagNo == _command.GetTagNo());
            Assert.AreEqual(_command.AreaCode, duplicatedTag.AreaCode);
            Assert.AreEqual(_areaDescription, duplicatedTag.AreaDescription);
            Assert.IsNull(duplicatedTag.Calloff);
            Assert.IsNull(duplicatedTag.CommPkgNo);
            Assert.AreEqual(_command.DisciplineCode, duplicatedTag.DisciplineCode);
            Assert.AreEqual(_disciplineDescription, duplicatedTag.DisciplineDescription);
            Assert.AreEqual(_command.TagType, duplicatedTag.TagType);
            Assert.IsNull(duplicatedTag.McPkgNo);
            Assert.AreEqual(_command.Description, duplicatedTag.Description);
            Assert.AreEqual(_command.Remark, duplicatedTag.Remark);
            Assert.AreEqual(_command.StorageArea, duplicatedTag.StorageArea);
            Assert.IsNull(duplicatedTag.PurchaseOrderNo);
            Assert.AreEqual(TestPlant, duplicatedTag.Plant);
            Assert.AreEqual(_stepId, duplicatedTag.StepId);
            Assert.IsNull(duplicatedTag.TagFunctionCode);
            Assert.AreEqual(_command.GetTagNo(), duplicatedTag.TagNo);
            Assert.AreEqual(2, duplicatedTag.Requirements.Count);
            AssertReqProperties(duplicatedTag.Requirements.First(), _rdId1, _interval1);
            AssertReqProperties(duplicatedTag.Requirements.Last(), _rdId2, _interval2);
        }

        [TestMethod]
        public async Task HandlingDuplicateAreaTagCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        private void AssertReqProperties(TagRequirement req, int reqDefId, int interval)
        {
            Assert.AreEqual(reqDefId, req.RequirementDefinitionId);
            Assert.AreEqual(interval, req.IntervalWeeks);
        }
    }
}
